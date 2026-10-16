using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;

namespace ByMyPc.Postgresql.Repository.Intefaces
{
    public interface IRamRepo
    {
        ValueTask<Guid> CreateAsync(RamCreateModel model);
        Task<IEnumerable<RamDbModel>> GetByFilterAsync(RamFilterModel filterModel, CancellationToken cancellationToken);
        Task<IEnumerable<RamSmallModel>> GetByFilterWithPagAsync(RamFilterModel filterModel, int page, int pageSize, CancellationToken cancellationToken);
        Task<RamDbModel?> GetByIdAsync(Guid id);
        IAsyncEnumerable<RamDbModel> GetFullAsyncEnumerable(CancellationToken cancellationToken);
        Task<IEnumerable<RamDbModel>> GetFullWithPagAsync(int page, int pageSize, CancellationToken cancellationToken);
        IAsyncEnumerable<RamSmallModel> GetSmallAsyncEnumerable(CancellationToken cancellationToken);
        Task<IEnumerable<RamSmallModel>> GetSmallWithPagAsync(int page, int pageSize, CancellationToken cancellationToken);
        ValueTask<bool> RemoveAsync(Guid id);
        Task<IEnumerable<RamDbModel>> SearchByNameAsync(string name, CancellationToken cancellationToken);
        Task<IEnumerable<RamSmallModel>> SearchByNameWithPagAsync(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<RamDbModel?> UpdateAsync(RamUpdateModel model);
    }
}