using ByMyPC.Models.GPUModels.DTO;
using ByMyPC.Models.GPUModels.RDTO;

namespace ByMyPC.Services.GPUService
{
    public interface IGPUService
    {
        ValueTask<Guid> CreateAsync(DTOGPUCreateModel model);
        Task<IEnumerable<RDTOGPUModel>> GetByFilterAsync(DTOGPUFilter filter, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOGPUSmallModel>> GetByFilterWithPagAsync(DTOGPUFilter filter, int page, int pageSize, CancellationToken cancellationToken);
        Task<RDTOGPUModel?> GetByIDAsync(Guid id);
        Task<IEnumerable<RDTOGPUModel>> GetFullModelsAsync(CancellationToken cancellation);
        Task<IEnumerable<RDTOGPUSmallModel>> GetSmallModelsAsync(CancellationToken cancellation);
        Task<IEnumerable<RDTOGPUSmallModel>?> GetSmallWithPag(int page, int pageSize, CancellationToken cancellationToken);
        Task<bool> RemoveAsync(Guid id);
        Task<IEnumerable<RDTOGPUModel>?> SearchByNameAsync(string name, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOGPUSmallModel>?> SearchByNameAsyncWithPag(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<RDTOGPUModel?> UpdateAsync(DTOGPUUpdateModel model);
    }
}