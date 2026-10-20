using ByMyPc.Postgresql.Models;

namespace ByMyPC.Models.GPUModels.DTO
{
    public record DTOGPUUpdateModel
    (
        Guid ID,
        string? Name,
        int? VideoMemorySize,
        VideoSlots? VideoSlot,
        int? MemoryBus,
        string? TypeConnector,
        string? TypeMemory
    )
    { }

    public record DTOGPUCreateModel(string name,
                                    int videoMemorySize,
                                    VideoSlots VideoSlots,
                                    int memoryBus,
                                    string typeConnector,
                                    string typeMemory);

    public record DTOGPUFilter(Guid ID,
        string? Name,
        int? VideoMemorySize,
        VideoSlots? VideoSlot,
        int? MemoryBus,
        string? TypeConnector,
        string? TypeMemory);

}

