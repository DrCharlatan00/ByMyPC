using ByMyPc.Postgresql.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.Operation
{
    public class PSUUpdateModel
    {
        public PSUUpdateModel(Guid id, string? name, int? powerWatt, bool isLive, PSU_SIZE? size, bool? isModular, bool? isCertified)
        {
            this.id = id;
            Name = name;
            PowerWatt = powerWatt;
            IsLive = isLive;
            Size = size;
            IsModular = isModular;
            IsCertified = isCertified;
        }
        public PSUUpdateModel()
        {
            
        }

        public required Guid id { get; set; }
        public string? Name { get; set; }
        public int? PowerWatt { get; set; }
        public bool IsLive { get; set; }
        public PSU_SIZE? Size { get; set; }
        public bool? IsModular { get; set; }
        public bool? IsCertified { get; set; }

    }

    public class PSUCreateModel
    {
        public PSUCreateModel()
        {
            
        }
        public PSUCreateModel(string name, int powerWatt, bool isLive, PSU_SIZE size, bool isModular, bool isCertified)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            PowerWatt = powerWatt;
            IsLive = isLive;
            Size = size;
            IsModular = isModular;
            IsCertified = isCertified;
        }

        public string Name { get; set; }
        public int PowerWatt { get; set; } = 0;
        public bool IsLive { get; set; } = false;
        public PSU_SIZE Size { get; set; } = PSU_SIZE.UNKNOWN;
        public bool IsModular { get; set; } = false;
        public bool IsCertified { get; set; } = false;

    }
}
