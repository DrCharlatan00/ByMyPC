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
        public string Operation { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string RequestHash { get; set; } = string.Empty;
        public Guid ResourceId { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    }
}
