using Confluent.Kafka;
using Application.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Domain.Events;
using Microsoft.Extensions.Options;

namespace Infrastructure.Kafka
{
    public class KafkaC : BackgroundService
    {

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly KafkaSettings _kafkaSettings;


        public KafkaC(IServiceScopeFactory scopeFactory, IOptions<KafkaSettings> KafkaOptions)
        {
            _scopeFactory = scopeFactory;
            _kafkaSettings = KafkaOptions.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers,
                GroupId = "transaction-status-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_kafkaSettings.TransactionStatusTopic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine("Esperando mensaje de Kafka...");

                    var cr = consumer.Consume(TimeSpan.FromMilliseconds(500));
                    if (cr == null)
                    {
                        await Task.Delay(500, stoppingToken);
                        continue;
                    }
                    Console.WriteLine($"Mensaje recibido: {cr.Message.Value}");
                    var message = JsonSerializer.Deserialize<TransactionStatusMessage>(cr.Message.Value);


                    if (message != null)
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var repositorio = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

                        await repositorio.UpdateTransactionAsync(message.TransactionId, message.Status);
                        Console.WriteLine($"Transacción {message.TransactionId} actualizada a {message.Status}");
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {

                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }



    }
}