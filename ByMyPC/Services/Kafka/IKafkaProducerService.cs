namespace ByMyPC.Services.Kafka
{
    public interface IKafkaProducerService
    {
        Task SendMessageAsync(string topic, string key, string message, CancellationToken cancellationToken);
        Task SendMessageAsync(string topic, string key, string message);
    }
}