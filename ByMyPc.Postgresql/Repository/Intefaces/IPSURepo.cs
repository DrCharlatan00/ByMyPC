using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;

namespace ByMyPc.Postgresql.Repository.Intefaces
{
    public interface IPSURepo
    {
        Task<Guid> CreateAsync(PSUCreateModel model);
        Task<IEnumerable<PSUDbModel>> GetByFilterFull(PSUFilterModel filterModel, CancellationToken cancellation);
        Task<IEnumerable<PSUSmallModel>> GetByFilterSmall(PSUFilterModel filterModel, CancellationToken cancellation);
        Task<PSUDbModel?> GetByIdAsync(Guid id);
        IAsyncEnumerable<PSUDbModel> GetFullAsyncEnumerable(CancellationToken cancellation);
        Task<IEnumerable<PSUDbModel>> GetFullWithPag(int page, int pageSize, CancellationToken cancellation);
        IAsyncEnumerable<PSUSmallModel> GetSmallAsyncEnumerable(CancellationToken cancellation);
        Task<IEnumerable<PSUSmallModel>> GetSmallModelsAsyncWithPag(int page, int pageSize, CancellationToken cancellation);
        ValueTask<bool> RemoveAsync(Guid id);
        IAsyncEnumerable<PSUDbModel> SearchByNameAsyncEnumerable(string name, CancellationToken cancellationToken);
        IAsyncEnumerable<PSUSmallModel> SearchByNameSmallAsyncEnumerable(string name, CancellationToken cancellation);
        Task<PSUDbModel?> UpdateAsync(PSUUpdateModel model);
    }
}