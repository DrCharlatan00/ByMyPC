using ByMyPc.Postgresql.Models;

namespace ByMyPC.Models.PSUModels
{
    public class DTOPSUFilterModel
    {
        public DTOPSUFilterModel()
        {
            
        }
        public DTOPSUFilterModel(string? name, int? powerWatt, bool? isLive, PSU_SIZE? size, bool? isModular, bool? isСertified)
        {
            Name = name;
            PowerWatt = powerWatt;
            IsLive = isLive;
            Size = size;
            IsModular = isModular;
            IsCertified = isСertified;
        }

        public string? Name { get; set; } = null;
        public int? PowerWatt { get; set; } = null;
        public bool? IsLive { get; set; } = null;
        public PSU_SIZE? Size { get; set; } = null;
        public bool? IsModular { get; set; } = null;
        public bool? IsCertified { get; set; } = null;
    }
}
