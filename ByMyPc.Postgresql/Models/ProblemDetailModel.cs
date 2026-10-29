using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.Models
{
    public class ProblemDetailModel
    {
        public Guid ID { get; set; }

        public string NameException { get; set; } = string.Empty;

        public string NameAPIOrServer { get; set; } = string.Empty;

        public string Details { get; set; } = string.Empty;

        public DateTimeOffset TimeProblemCreated { get; set; }
    }
}
