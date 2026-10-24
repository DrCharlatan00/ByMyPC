namespace ByMyPC.Models
{
    public record KafkaEvent(
            string eventType,
            Guid componentID,
            string name,
            DateTimeOffset time
        );
    
}
