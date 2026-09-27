using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.Models
{
    public  class PSUDbModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = "N/A";
        public int PowerWatt { get; set; } = 0;
        public bool IsLive { get; set; } = false;
        public PSU_SIZE Size { get; set; } = PSU_SIZE.UNKNOWN;
        public bool IsModular { get; set; } = false;
        public bool IsСertified { get; set; } = false;


    }

    public enum PSU_SIZE
    {
        UNKNOWN = 0,
        ATX = 1,
        FLEX_ATX = 2,
        SFX = 3,
        TFX = 4,
    }
}
