using ByMyPc.Postgresql.Models;

namespace ByMyPC.Models.RAMModels.RDTO
{
    public record RDTORAMModel(Guid id, string name, int size, TypeDDR Type, int frequency, bool isLive);

    public record RDTORAMSmallModel(Guid id, string name, int size, TypeDDR Type);

}
