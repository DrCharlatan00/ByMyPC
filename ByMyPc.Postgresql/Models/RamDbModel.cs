using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.Models
{
    public class RamDbModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = "N/A";
        public TypeDDR DDRType { get; set; } = TypeDDR.DDR;
        public int Frequency { get; set; } = 0;
        public bool IsLive { get; set; } = false;
    }

    public enum TypeDDR {
        DDR = 1,
        DDR2 = 2,
        DDR3 = 3,
        DDR4 = 4,
        DDR5 = 5
    }
}
