using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;

namespace ByMyPc.Postgresql.Repository.Intefaces
{
    public interface IGPURepo
    {
        Task<Guid> CreateAsync(GPUCreateModel model);
        Task<IEnumerable<GpuDbModel>> GetByFilter(GPUFilterModel filterModel, CancellationToken cancellationToken);
        Task<GpuDbModel?> GetByID(Guid id);
        IAsyncEnumerable<GpuDbModel> GetFullModelsDbAsync(CancellationToken cancellationToken);
        Task<IEnumerable<GPUSmallModel>> GetSmallByFilter(GPUFilterModel filterModel, int page, int pageSize, CancellationToken cancellationToken);
        IAsyncEnumerable<GPUSmallModel> GetSmallModelsDbAsync(CancellationToken cancellationToken);
        Task<IEnumerable<GPUSmallModel>> GetSmallModelWithPagination(int page, int pageSize, CancellationToken cancellationToken);
        ValueTask<bool> RemoveAsync(Guid id);
        Task<IEnumerable<GpuDbModel>?> SearchByName(string name, CancellationToken cancellationToken);
        Task<IEnumerable<GPUSmallModel>?> SearchByNameWithPag(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<GpuDbModel?> UpdateAsync(GPUUpdateModel model);
    }
}