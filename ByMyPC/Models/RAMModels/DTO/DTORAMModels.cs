using ByMyPc.Postgresql.Models;

namespace ByMyPC.Models.RAMModels.DTO
{
    public record DTORAMCreate(string name, int size, TypeDDR Type, int frequency, bool isLive);
    public record DTORAMUpdate(Guid id,string? name, int? size, TypeDDR? Type, int? frequency, bool? isLive);

}
