namespace ByMyPC.Models.CpuModels.DTO
{
    public record DTOCpuCreateModel(string Name, string Socket, int Frequency, int Count_Cores, bool IsLive);
    public record DTOCpuUpdateModel(Guid id,string? Name, string? Socket, int Frequency, int Count_Cores, bool IsLive);

}
