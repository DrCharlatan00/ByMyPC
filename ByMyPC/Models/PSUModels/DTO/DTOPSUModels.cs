using ByMyPc.Postgresql.Models;

namespace ByMyPC.Models.PSUModels.DTO
{
    public record DTOPSUModelCreate(string Name,
                                    int PowerWatt,
                                    bool IsLive,
                                    PSU_SIZE Size,
                                    bool IsModular,
                                    bool IsCertified);
    public record DTOPSUModelUpdate(Guid id,
                                    string? Name,
                                    int? PowerWatt,
                                    bool IsLive,
                                    PSU_SIZE? Size,
                                    bool? IsModular,
                                    bool? IsCertified);

}
