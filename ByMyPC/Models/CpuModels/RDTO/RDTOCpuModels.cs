namespace ByMyPC.Models.CpuModels.RDTO
{
    public record RDTOCpuModel(Guid id,string Name, string Socket, int Frequency, int Count_Cores, bool IsLive);

    public record RDTOCpuSmallModel(string Name, string Socket);
   
}
