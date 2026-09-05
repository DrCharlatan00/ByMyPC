using ByMyPc.Postgresql.CRUDModel.FiltersModels;

namespace ByMyPC.Models.CpuModels.DTO
{
    public record DTOCpuCreateModel(string Name, string Socket, int Frequency, int Count_Cores, bool IsLive);
    public record DTOCpuUpdateModel(Guid id,string? Name, string? Socket, int Frequency, int Count_Cores, bool IsLive);
    public class DTOCpuFilter
    {
        public string? ByName { get; set; } = null;
        public bool? ByLive { get; set; } = null;
        public int? ByQuantityCores { get; set; } = null;

        public CPUFilterModel ConvertToDbFilter(DTOCpuFilter filter) {
            return new CPUFilterModel {
                ByLive = filter.ByLive,
                ByName = filter.ByName,
                ByQuantityCores = filter.ByQuantityCores,
            };
        }
    }


}
