using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.Models
{
    public class OutboxMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid(); //PK
        public string Topic { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty; // Type Event ex. Created Updated Removed

        public string AggregateId { get; set; } = string.Empty; // Id Entity for created OutBox Mess

        public string Payload { get; set; } = string.Empty; // JSON With data Message

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // date time created event with utc

        public DateTime? ProcessedAt { get; set; } // UTC time when the event was successfully published

        public int RetryCount { get; set; }
    }
}
