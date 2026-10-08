using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.FiltersModels
{
    public class CPUFilterModel
    {
        public CPUFilterModel(string? byName, bool? byLive, int? byQuantityCores)
        {
            ByName = byName;
            ByLive = byLive;
            ByQuantityCores = byQuantityCores;
        }

        public string? ByName { get; set; } = null;
        public bool? ByLive { get; set; } = null;
        public int? ByQuantityCores { get; set; } = null;
    }
}
