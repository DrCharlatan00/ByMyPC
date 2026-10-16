using ByMyPc.Postgresql.Models;

namespace ByMyPC.Models.RAMModels.DTO
{
    public record DTORAMFilter(string? name, int? size, TypeDDR? Type, int? frequency, bool? isLive);

}
