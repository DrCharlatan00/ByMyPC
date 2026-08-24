using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.Models
{
    public class PcHddDbModel
    {
        public Guid PcId { get; set; }

        public PcDbModel Pc { get; set; } = null!;


        public Guid HddId { get; set; }

        public HDDDbModel Hdd { get; set; } = null!;
    }
}
