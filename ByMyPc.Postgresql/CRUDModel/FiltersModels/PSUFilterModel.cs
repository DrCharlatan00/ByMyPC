using ByMyPc.Postgresql.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.FiltersModels
{
    public class PSUFilterModel
    {
        public PSUFilterModel()
        {
            
        }
        public PSUFilterModel(string? name, int? powerWatt, bool? isLive, PSU_SIZE? size, bool? isModular, bool? isСertified)
        {
            Name = name;
            PowerWatt = powerWatt;
            IsLive = isLive;
            Size = size;
            IsModular = isModular;
            IsСertified = isСertified;
        }

        public string? Name { get; set; } = null;
        public int? PowerWatt { get; set; } = null;
        public bool? IsLive { get; set; } = null;
        public PSU_SIZE? Size { get; set; } =  null;
        public bool? IsModular { get; set; } =  null;
        public bool? IsСertified { get; set; } =  null;
    }
}
