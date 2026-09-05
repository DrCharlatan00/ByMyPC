using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;

namespace ByMyPc.Postgresql.Repository.Intefaces
{
    public interface ICpuRepo
    {
        Task<Guid> CreateAsync(CpuCreateModel model);
        Task<IEnumerable<CpuDbModel>> GetAsyncPagination(int page, int pageSize, CancellationToken cancellationToken);
        Task<CpuDbModel?> GetByIDAsync(Guid id);
        IAsyncEnumerable<CpuDbModel> GetCpuRepoAsyncEnumerable(CancellationToken cancellationToken);
        Task<IEnumerable<CpuSmallModel>> GetCpuSmallModelsPagination(int page, int pageSize, CancellationToken cancellationToken);
        IAsyncEnumerable<CpuSmallModel> GetCpuSmallRepoAsyncEnumerable(CancellationToken cancellationToken);
        Task RemoveAsync(Guid id);
        IAsyncEnumerable<CpuSmallModel> SearchCpuSmallByNameAsyncEnumerable(string name, CancellationToken cancellationToken);
        IAsyncEnumerable<CpuSmallModel> SearchCpuSmallByNameWithPaginationAsyncEnumerable(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<CpuDbModel?> UpdateAsync(Guid id, CpuUpdateModel model);
        Task<IEnumerable<CpuDbModel>> GetByFilterWithPagAsync(CPUFilterModel filterModel, int page, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<CpuDbModel>> GetByFilterAsync(CPUFilterModel filterModel, CancellationToken cancellationToken);
    }
}