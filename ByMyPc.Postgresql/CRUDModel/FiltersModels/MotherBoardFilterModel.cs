using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.FiltersModels
{
    public class MotherBoardFilterModel
    {
        public string? ByName { get; set; } = null;
        public bool? ByLive { get; set; } = null;
        public string? BySocket { get; set; } = null;
        public bool? ByHaveIntegratedGPU { get; set; } = null;
    }
}
