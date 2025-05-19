using AntiFraudService.config;
using Confluent.Kafka;
using Domain.Events;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Net.Http.Json;

namespace AntiFraudService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IHttpClientFactory _httpClientFactory;

    public Worker(ILogger<Worker> logger, IOptions<KafkaSettings> KafkaOptions, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _kafkaSettings = KafkaOptions.Value;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = "antifraud-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

        consumer.Subscribe(_kafkaSettings.ConsumeTopic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cr = consumer.Consume(stoppingToken);
                var transaction = JsonSerializer.Deserialize<TransactionCreatedMessage>(cr.Message.Value);

                if (transaction == null) continue;

                // Consulta el acumulado diario real desde la API principal
                var httpClient = _httpClientFactory.CreateClient();
                // Cambia la URL y puerto según tu docker-compose (ejemplo: "api" es el nombre del servicio)
                var url = $"http://localhost:5012/api/transactions/daily-total/{transaction.SourceAccountId}";
                decimal dailyTotal = 0;
                try
                {
                    var response = await httpClient.GetAsync(url, stoppingToken);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<DailyTotalResponse>(cancellationToken: stoppingToken);
                        dailyTotal = result?.total ?? 0;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error consultando daily total: {ex.Message}");
                }

                dailyTotal += transaction.Value;

                string status = "Approved";
                if (transaction.Value > 2000 || dailyTotal > 20000)
                    status = "Rejected";

                var statusMessage = new TransactionStatusMessage
                {
                    TransactionId = transaction.Id,
                    Status = status == "Approved" ? Domain.Entities.TransactionStatus.Approved : Domain.Entities.TransactionStatus.Rejected
                };

                var json = JsonSerializer.Serialize(statusMessage);
                await producer.ProduceAsync(_kafkaSettings.ProduceTopic, new Message<Null, string> { Value = json });
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _logger.LogError($"AntiFraud error: {ex.Message}");
            }
        }
    }

    // DTO para la respuesta del endpoint
    private class DailyTotalResponse
    {
        public decimal total { get; set; }
    }
}