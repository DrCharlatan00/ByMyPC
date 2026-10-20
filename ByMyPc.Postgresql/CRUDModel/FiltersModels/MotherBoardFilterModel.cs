using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.FiltersModels
{
    public class MotherBoardFilterModel
    {
        public MotherBoardFilterModel(string? byName, bool? byLive, string? bySocket, bool? byHaveIntegratedGPU)
        {
            ByName = byName;
            ByLive = byLive;
            BySocket = bySocket;
            ByHaveIntegratedGPU = byHaveIntegratedGPU;
        }

        public string? ByName { get; set; } = null;
        public bool? ByLive { get; set; } = null;
        public string? BySocket { get; set; } = null;
        public bool? ByHaveIntegratedGPU { get; set; } = null;
    }
}
