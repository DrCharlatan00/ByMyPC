using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.SmallModels
{
    public class GPUSmallModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = "N/A";
        public int VideoMemorySize { get; set; } = 0;

    }
}
