using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ByMyPc.Postgresql.Models
{
    internal class PayloadKafka
    {
        public PayloadKafka()
        {
        }

        public PayloadKafka(string eventType, Guid componentID, string name, DateTimeOffset occurredAt)
        {
            this.eventType = eventType;
            ComponentID = componentID;
            this.name = name;
            this.occurredAt = occurredAt;
        }

        public string eventType { get; set; } = string.Empty;
        public Guid ComponentID { get; set; }
        public string name { get; set; }
        public DateTimeOffset occurredAt { get; set; }

        public string Convert2Json() {
            return JsonSerializer.Serialize(this);
        }
    }
}
