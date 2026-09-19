using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.SmallModels
{
    public record HDDSmallModel(Guid id,string Name = "n?a", int GbSize = 0);
}
