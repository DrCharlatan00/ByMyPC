using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ByMyPc.Postgresql.Models
{
    public class IdempotencyRecord
    {
        public Guid ID { get; set; }
        public string Operation { get; set; } = string.Empty; // What as a Operation
        public string Key { get; set; } = string.Empty; // Key HTTP requst client
        public string RequestHash { get; set; } = string.Empty; // what we change
        public Guid ResourceId { get; set; } // ID Component
        public DateTime CreateAt { get; set; } = DateTime.UtcNow; 
    }
}
