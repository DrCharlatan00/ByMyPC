using ByMyPc.Postgresql.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.Operation
{
    public class GPUCreateModel
    {
        public GPUCreateModel()
        {
            
        }

        public GPUCreateModel(string name, int videoMemorySize, VideoSlots videoSlot, int memoryBus, string typeConnector, string typeMemory)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            VideoMemorySize = videoMemorySize;
            VideoSlot = videoSlot;
            MemoryBus = memoryBus;
            TypeConnector = typeConnector ?? throw new ArgumentNullException(nameof(typeConnector));
            TypeMemory = typeMemory ?? throw new ArgumentNullException(nameof(typeMemory));
        }

        public string Name { get; set; } = "N/A";
        public int VideoMemorySize { get; set; } = 0;
        public VideoSlots VideoSlot { get; set; } = VideoSlots.UNKNOWN;
        public int MemoryBus { get; set; } = 0;
        public string TypeConnector { get; set; } = string.Empty;
        public string TypeMemory { get; set; } = string.Empty;
    }
}
