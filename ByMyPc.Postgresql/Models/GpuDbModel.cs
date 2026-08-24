using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.Models
{
    public class GpuDbModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = "N/A";
        public int VideoMemorySize { get; set; } = 0;
        public VideoSlots VideoSlot { get; set; } = VideoSlots.UNKNOWN;
    }
}
