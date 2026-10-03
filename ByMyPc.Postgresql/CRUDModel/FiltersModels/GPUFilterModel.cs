using ByMyPc.Postgresql.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.FiltersModels
{
    public class GPUFilterModel
    {
        public GPUFilterModel()
        {

        }

        public GPUFilterModel(Guid iD, string? name, int? videoMemorySize, VideoSlots? videoSlot, int? memoryBus, string? typeConnector, string? typeMemory)
        {
            ID = iD;
            Name = name;
            VideoMemorySize = videoMemorySize;
            VideoSlot = videoSlot;
            MemoryBus = memoryBus;
            TypeConnector = typeConnector;
            TypeMemory = typeMemory;
        }

        public Guid ID { get; set; }
        public string? Name { get; set; } = null;
        public int? VideoMemorySize { get; set; } = null;
        public VideoSlots? VideoSlot { get; set; } = null;
        public int? MemoryBus { get; set; } = null;
        public string? TypeConnector { get; set; } = null;
        public string? TypeMemory { get; set; } = null;
    }
}
