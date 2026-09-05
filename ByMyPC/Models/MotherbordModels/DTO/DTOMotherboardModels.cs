using ByMyPc.Postgresql.CRUDModel.FiltersModels;

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

    public class DTOMotherboardFilter
    {
        public string? ByName { get; set; } = null;
        public bool? ByLive { get; set; } = null;
        public string? BySocket { get; set; } = null;
        public bool? ByHaveIntegratedGPU { get; set; } = null;

        public MotherBoardFilterModel ConvertToDbFilter(DTOMotherboardFilter filter) {
            return new MotherBoardFilterModel {
                ByHaveIntegratedGPU = filter.ByHaveIntegratedGPU,
                ByLive = filter.ByLive,
                ByName = filter.ByName,
                BySocket = filter.BySocket,
            };
        }
    }
}
