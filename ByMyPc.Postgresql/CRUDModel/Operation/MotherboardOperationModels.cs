using ByMyPc.Postgresql.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.Operation
{
    public class MotherboardUpdateModel
    {
        public MotherboardUpdateModel()
        {
            
        }
        public MotherboardUpdateModel(Guid iD, string name, string socket, bool isLive)
        {
            ID = iD;
            Name = name;
            Socket = socket;
            IsLive = isLive;
        }

        public Guid ID { get; set; }
        public string Name { get; set; } = "N/A";
        public string Socket { get; set; } = "N?A";
        public bool IsLive { get; set; } = false;

    }

    public class MotherboardCreateModel
    {
        public MotherboardCreateModel()
        {
            
        }
        public MotherboardCreateModel(string name, string socket, int maxRamSlot, int maxRamFrequency, int maxCpuFrequency, bool integrationGpu, bool isLive, VideoSlots videoSlot)
        {
            Name = name;
            Socket = socket;
            MaxRamSlot = maxRamSlot;
            MaxRamFrequency = maxRamFrequency;
            MaxCpuFrequency = maxCpuFrequency;
            IntegrationGpu = integrationGpu;
            IsLive = isLive;
            VideoSlot = videoSlot;
        }

        public string Name { get; set; } = "N/A";
        public string Socket { get; set; } = "N?A";
        public int MaxRamSlot { get; set; } = 0;
        public int MaxRamFrequency { get; set; } = 0;
        public int MaxCpuFrequency { get; set; } = 0;
        public bool IntegrationGpu { get; set; } = false;
        public bool IsLive { get; set; } = false;
        public VideoSlots VideoSlot { get; set; } = VideoSlots.UNKNOWN;
    }
}
