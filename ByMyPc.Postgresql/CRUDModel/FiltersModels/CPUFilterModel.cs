using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.FiltersModels
{
    public class CPUFilterModel
    {
        public string? ByName { get; set; } = null;
        public bool? ByLive { get; set; } = null;
        public int? ByQuantityCores { get; set; } = null;
    }
}
