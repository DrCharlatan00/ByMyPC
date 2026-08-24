using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.Models
{
    public class PcRamDbModel
    {
        public Guid PcId { get; set; }

        public PcDbModel Pc { get; set; } = null!;


        public Guid RamId { get; set; }

        public RamDbModel Ram { get; set; } = null!;


        public int Slot { get; set; }
    }
}
