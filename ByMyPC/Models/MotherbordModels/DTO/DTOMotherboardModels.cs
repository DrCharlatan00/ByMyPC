namespace ByMyPC.Models.MotherbordModels.DTO
{
    public record DTOMotherboardCreateModel(string Name,
                                            string Socket,
                                            int MaxRamSlot,
                                            int MaxRamFrequency,
                                            int MaxCpuFrequency,
                                            bool IntegrationGpu,
                                            bool IsLive,
                                            RDTO.VideoSlots VideoSlot);

    public record DTOMotherboardUpdateModel(Guid id,string Name, string Socket, bool IsLive);
}
