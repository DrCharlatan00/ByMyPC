using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.Models
{
    public class PcDbModel
    {
        public Guid ID { get; set; }

        public string Name { get; set; } = "N/A";

        public Guid? CpuId { get; set; }
        public CpuDbModel? Cpu { get; set; }

        public Guid? GpuId { get; set; }
        public GpuDbModel? Gpu { get; set; }

        public Guid? MotherboardId { get; set; }
        public MotherboardDbModel? Motherboard { get; set; }

        public Guid? PSUId { get; set; }
        public PSUDbModel? PSU { get; set; }

        public ICollection<PcRamDbModel> Rams { get; set; } = new List<PcRamDbModel>();

        public ICollection<PcHddDbModel> HDDs { get; set; } = new List<PcHddDbModel>();

    }
}
