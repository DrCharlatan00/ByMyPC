namespace ByMyPC.Models.MotherbordModels.RDTO
{
    public record RDTOModelMotherboardCard(string Name, string Socket, bool IsLive);
    public record RDTOModelMotherboard(Guid id, string Name, string Socket, int MaxRamSlot, int MaxRamFrequency, int MaxCpuFrequency, bool IntegrationGpu, bool IsLive, VideoSlots VideoSlot);

    public enum VideoSlots
    {
        UNKNOWN = 0,
        AGP = 1,
        PCI_E = 2
    }

}
