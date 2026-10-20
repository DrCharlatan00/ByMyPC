using ByMyPc.Postgresql.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ByMyPC.Models.PSUModels.RDTO
{
    public record RDTOPSUModel(Guid id, string Name, int PowerWatt, bool IsLive, PSU_SIZE Size, bool IsModular, bool IsСertified);

    public record RDTOPSUSmallModel(Guid id, string Name, int PowerWatt, bool IsLive);


}
