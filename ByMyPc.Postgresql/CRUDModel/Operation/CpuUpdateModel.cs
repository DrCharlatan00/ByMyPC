using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.Operation
{
    public class CpuUpdateModel
    {
        public string? Name { get; set; }
        public string? Socket { get; set; }
        public int Frequency { get; set; }
        public int Count_Cores { get; set; }
        public bool IsLive { get; set; }
    }

    public class CpuCreateModel
    {
        public string Name { get; set; } = "N/A";
        public string Socket { get; set; } = "N?A";
        public int Frequency { get; set; } = 0;
        public int Count_Cores { get; set; } = 0;
        public bool IsLive { get; set; } = false;
    }
}
