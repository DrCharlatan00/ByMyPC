namespace ByMyPC.Models.PCModels.DTO
{
    public record DTOPCUpdateModel(Guid id,string name);

    public record DTOPCCreateModel(string name);

    public record DTOPCAttach(Guid pc, Guid Item, WithAttachToPc WithAttachToPcCode);

    public enum WithAttachToPc
    {
        CPU = 0,
        GPU = 1,
        MOTHERBOARD = 2,
        PSU = 3
    }

    public record DTOPCFilter(string? name, int? countCore, int? videoMemorySize, string? socket, int? powerWatt);
}
