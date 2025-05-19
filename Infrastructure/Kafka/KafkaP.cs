using Confluent.Kafka;
using Application.Interfaces;
using System.Text.Json;

namespace Infrastructure.Kafka
{
    public class KafkaP : IKafka
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaP()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task Kafka(string topic, object message)
        {

            var json = JsonSerializer.Serialize(message);

            await _producer.ProduceAsync(topic, new Message<Null, string>
            {
                Value = json
            });

        }



    }
}