using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.FiltersModels
{
    public class PcFilterModel
    {
        public PcFilterModel()
        {
            
        }

        public PcFilterModel(string? namePc, int? countCore, int? videoMemorySize, string? socket, int? powerWatt)
        {
            this.namePc = namePc;
            this.countCore = countCore;
            this.videoMemorySize = videoMemorySize;
            this.socket = socket;
            this.powerWatt = powerWatt;
        }

        public string? namePc { get; set; } = null;
        public int? countCore { get; set; } = null;
        public int? videoMemorySize { get; set; } = null;
        public string? socket { get; set; } = null;

        public int? powerWatt { get; set; } = null;
    }
}
