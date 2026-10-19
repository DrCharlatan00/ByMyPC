using ByMyPc.Postgresql.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.FiltersModels
{
    public class RamFilterModel
    {
        public RamFilterModel()
        {
                
        }

        public RamFilterModel(string? name, int? size, TypeDDR? dDRType, int? frequency, bool? isLive)
        {
            Name = name;
            Size = size;
            DDRType = dDRType;
            Frequency = frequency;
            IsLive = isLive;
        }

        public string? Name { get; set; } = null;
            public int? Size { get; set; } = null;
            public TypeDDR? DDRType { get; set; } = null;
            public int? Frequency { get; set; } = null;
            public bool? IsLive { get; set; } = null;
        
    }
}
