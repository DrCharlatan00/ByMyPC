using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.Operation
{
    public class PcUpdateModel
    {
        public PcUpdateModel(Guid iD, string name)
        {
            ID = iD;
            Name = name;
        }

        public Guid ID { get; init; }
        public string Name { get; init; } = string.Empty;
    }

    public class PcCreateModel
    {
        public PcCreateModel(string name)
        {
            Name = name;
        }

        public string Name { get; init; } = string.Empty;
    }
}
