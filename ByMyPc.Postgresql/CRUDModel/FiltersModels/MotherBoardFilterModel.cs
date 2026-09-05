using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.FiltersModels
{
    internal class MotherBoardFilterModel
    {
        public string? ByName { get; set; } = null;
        public string? ByLive { get; set; } = null;
        public string? BySocket { get; set; } = null;
        public string? ByHaveIntegratedGPU { get; set; } = null;
    }
}
