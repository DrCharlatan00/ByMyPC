using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.Models
{
    public  class HDDDbModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = "N/A";
        public int GbSize { get; set; } = 0;
        public HddConnector connector { get; set; } = HddConnector.UNKNOWN;
    }

    public enum HddConnector {
        UNKNOWN = 0,
        IDE = 1,
        SATA = 2,
    }
}
