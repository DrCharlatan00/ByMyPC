
using System.Runtime.CompilerServices;

namespace ByMyPc.Postgresql.Models
{
    public class CpuDbModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = "N/A";
        public string Socket { get; set; } = "N?A";
        public int Frequency { get; set; } = 0;
        public int Count_Cores { get; set; } = 0;
        public bool IsLive { get; set; } = false;
    }
}
