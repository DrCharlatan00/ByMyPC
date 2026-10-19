using ByMyPc.Postgresql.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.SmallModels
{
    public class RamSmallModel
    {
        public RamSmallModel()
        {
                
        }
        public RamSmallModel(Guid iD, string name, int size, TypeDDR dDRType)
        {
            ID = iD;
            Name = name;
            Size = size;
            DDRType = dDRType;
        }

        public Guid ID { get; set; }
        public string Name { get; set; } = "N/A";
        public int Size { get; set; } = 8;
        public TypeDDR DDRType { get; set; } = TypeDDR.DDR;
    }
}
