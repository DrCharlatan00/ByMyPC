using Confluent.Kafka;

namespace ByMyPC.Services.Kafka
{
    public class KafkaProducerService : IDisposable, IKafkaProducerService
    {
        private readonly IProducer<string, string> producer;

        public KafkaProducerService(IConfiguration configuration)
        {
            var bootsrapServers = configuration["Kafka:BootsrapServers"] ?? "localhost:9094";

            var conf = new ProducerConfig
            {
                BootstrapServers = bootsrapServers,
            };

            this.producer = new ProducerBuilder<string, string>(conf).Build();
        }

        public async Task SendMessageAsync(string topic,
                                           string key,
                                           string message,CancellationToken cancellationToken = default)
        {
            var KafkaMess = new Message<string, string>
            {
                Key = key,
                Value = message
            };

            await producer.ProduceAsync(
                    topic,
                    KafkaMess,
                    cancellationToken
                );
        }

        public async Task SendMessageAsync(string topic,
                                       string key,
                                       string message)
        {
            var KafkaMess = new Message<string, string>
            {
                Key = key,
                Value = message
            };

            await producer.ProduceAsync(
                    topic,
                    KafkaMess
                );
        }

        public void Dispose()
        {
            producer.Dispose();
        }
    }
}
