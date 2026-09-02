using ByMyPC.Models.CpuModels.DTO;
using ByMyPC.Models.CpuModels.RDTO;

namespace ByMyPC.Services.CpuService
{
    public interface ICpuService
    {
        Task<Guid> CreateAsync(DTOCpuCreateModel model);
        Task<RDTOCpuModel?> GetById(Guid Id);
        Task<IEnumerable<RDTOCpuModel>> GetFullCpuAsync(CancellationToken cancellationToken);
        Task<IEnumerable<RDTOCpuModel>> GetFullCpuPagination(int page, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOCpuSmallModel>> GetRDTOSmallModelAsync(CancellationToken cancellationToken);
        Task<IEnumerable<RDTOCpuSmallModel>> GetSmallModelsWithPaginationAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task RemoveAsync(Guid id);
        Task<IEnumerable<RDTOCpuSmallModel>> SearchByNameAsync(string name, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOCpuSmallModel>> SearchByNameWithPaginationAsync(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<RDTOCpuModel?> UpdateAsync(DTOCpuUpdateModel model);
    }
}