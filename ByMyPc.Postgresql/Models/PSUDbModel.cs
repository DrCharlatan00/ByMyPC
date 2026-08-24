using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.Models
{
    public  class PSUDbModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = "N/A";
        public int PowerWatt { get; set; } = 0;
    }
}
