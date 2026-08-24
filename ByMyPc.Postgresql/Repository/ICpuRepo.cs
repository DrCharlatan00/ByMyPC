using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;

namespace ByMyPc.Postgresql.Repository
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
        IAsyncEnumerable<CpuSmallModel> SearchCpuSmallByNameAsyncEnumerable(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<(CpuDbModel? updatedModel, string Message)> UpdateAsync(Guid id, CpuUpdateModel model);
    }
}