using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.CRUDModel.SmallModels
{
    public record PSUSmallModel
    (
        Guid Id,
        string Name,
        int PowerWatt,
        bool IsLive
    );    
}
