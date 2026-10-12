using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;

namespace ByMyPc.Postgresql.Repository.Intefaces
{
    public interface IMotherboardRepo
    {
        ValueTask<Guid> CreateAsync(MotherboardCreateModel model);
        Task<MotherboardDbModel?> GetByIDAsync(Guid id);
        IAsyncEnumerable<MotherboardSmallDbModel> GetCardMotherboardDbAsync(CancellationToken cancellationToken);
        Task<IEnumerable<MotherboardSmallDbModel>> GetCardWithPaginationAsync(int page, int pageSize, CancellationToken cancellationToken);
        IAsyncEnumerable<MotherboardDbModel> GetFullMotherboardDbAsync(CancellationToken cancellationToken);
        ValueTask<bool> RemoveAsync(Guid id);
        Task<IEnumerable<MotherboardSmallDbModel>> SearchByNameMotherboardSmallAsync(string name, CancellationToken cancellationToken);
        Task<IEnumerable<MotherboardDbModel>> GetByFilterAsync(MotherBoardFilterModel filter, CancellationToken cancellationToken);
        Task<IEnumerable<MotherboardSmallDbModel>> GetByFilterWithPagAsync(MotherBoardFilterModel filter, int page, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<MotherboardSmallDbModel>> SearchByNameMotherboardSmallWithPaginationAsync(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<MotherboardDbModel?> UpdateAsync(MotherboardUpdateModel model);
        Task<IEnumerable<MotherboardDbModel>> GetFullWithPag(int page, int pageSize, CancellationToken cancellationToken);
    }
}