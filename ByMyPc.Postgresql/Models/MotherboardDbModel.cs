

namespace ByMyPc.Postgresql.Models
{
    public class MotherboardDbModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = "N/A";
        public string Socket { get; set; } = "N?A";
        public int MaxRamSlot { get; set; } = 0;
        public int MaxRamFrequency { get; set; } = 0;
        public int MaxCpuFrequency { get; set; } = 0;
        public bool IntegrationGpu { get; set; } = false;
        public bool IsLive { get; set; } = false;
        public VideoSlots VideoSlot { get; set; } = VideoSlots.UNKNOWN;
    }

    public enum VideoSlots {
        UNKNOWN = 0,
        AGP = 1,
        PCI_E = 2
    }
}
