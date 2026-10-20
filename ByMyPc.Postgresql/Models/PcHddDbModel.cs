using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.Models
{
    public class PcHddDbModel
    {
        public PcHddDbModel()
        {
            
        }

        public PcHddDbModel(Guid pcId, Guid hddId)
        {
            PcId = pcId;
            HddId = hddId;
        }

        public Guid PcId { get; set; }

        public PcDbModel Pc { get; set; } = null!;


        public Guid HddId { get; set; }

        public HDDDbModel Hdd { get; set; } = null!;
    }
}
