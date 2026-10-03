using ByMyPc.Postgresql.Models;

namespace ByMyPC.Models.GPUModels.RDTO
{
    public record RDTOGPUModel(Guid id,
                               string name,
                               int videoMemorySize,
                               VideoSlots VideoSlots,
                               int memoryBus,
                               string typeConnector,
                               string typeMemory);

    public record RDTOGPUSmallModel(
            Guid id,
            string name,
            int videoMemorySize
        );
}
