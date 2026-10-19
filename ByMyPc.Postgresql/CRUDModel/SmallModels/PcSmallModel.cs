using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.SmallModels
{
    public class PcSmallModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = "N/A";

        public Guid? CpuId { get; set; }

        public Guid? GpuId { get; set; }

        public Guid? MotherboardId { get; set; }

        public Guid? PSUId { get; set; }
        
        public IEnumerable<Guid> Rams { get; set; } = Enumerable.Empty<Guid>();
        public IEnumerable<Guid> Hdd { get; set; } = Enumerable.Empty<Guid>();

    }
}
