using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;

namespace ByMyPc.Postgresql.Repository.Intefaces
{
    public interface IMotherboardRepo
    {
        Task<Guid> CreateAsync(MotherboardCreateModel model);
        Task<MotherboardDbModel?> GetByIDAsync(Guid id);
        IAsyncEnumerable<MotherboardSmallDbModel> GetCardMotherboardDbAsync(CancellationToken cancellationToken);
        Task<IEnumerable<MotherboardSmallDbModel>> GetCardWithPaginationAsync(int page, int pageSize, CancellationToken cancellationToken);
        IAsyncEnumerable<MotherboardDbModel> GetFullMotherboardDbAsync(CancellationToken cancellationToken);
        Task RemoveAsync(Guid id);
        Task<IEnumerable<MotherboardSmallDbModel>> SearchByNameMotherboardSmallAsync(string name, CancellationToken cancellationToken);
        Task<IEnumerable<MotherboardSmallDbModel>> SearchByNameMotherboardSmallWithPaginationAsync(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<MotherboardDbModel?> UpdateAsync(MotherboardUpdateModel model);
    }
}