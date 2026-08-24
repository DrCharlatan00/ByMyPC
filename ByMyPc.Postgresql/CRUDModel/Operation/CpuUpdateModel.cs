using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.Operation
{
    public class CpuUpdateModel
    {
        public CpuUpdateModel(string? name, string? socket, int frequency, int count_Cores, bool isLive)
        {
            Name = name;
            Socket = socket;
            Frequency = frequency;
            Count_Cores = count_Cores;
            IsLive = isLive;
        }

        public string? Name { get; set; }
        public string? Socket { get; set; }
        public int Frequency { get; set; }
        public int Count_Cores { get; set; }
        public bool IsLive { get; set; }
    }

    public class CpuCreateModel
    {
        public CpuCreateModel(string name, string socket)
        {
            Name = name;
            Socket = socket;
        }

        public CpuCreateModel(string name, string socket, int frequency, int count_Cores, bool isLive)
        {
            Name = name;
            Socket = socket;
            Frequency = frequency;
            Count_Cores = count_Cores;
            IsLive = isLive;
        }

        public string Name { get; set; } = "N/A";
        public string Socket { get; set; } = "N?A";
        public int Frequency { get; set; } = 0;
        public int Count_Cores { get; set; } = 0;
        public bool IsLive { get; set; } = false;
    }
}
