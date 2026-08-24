using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.SmallModels
{
    public record CpuSmallModel
    (
         string Name = "N/A",
         string Socket = "N?A"
    );
}
