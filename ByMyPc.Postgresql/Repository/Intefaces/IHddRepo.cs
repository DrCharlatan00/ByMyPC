using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;

namespace ByMyPc.Postgresql.Repository.Intefaces
{
    public interface IHddRepo
    {
        Task<Guid> CreateAsync(HDDCreateModel createModel);
        Task<HDDDbModel?> GetByID(Guid id);
        IAsyncEnumerable<HDDDbModel> GetModelsDbAsync(CancellationToken cancellationToken);
        IAsyncEnumerable<HDDSmallModel> GetSmallModelsDbAsync(CancellationToken cancellationToken);
        Task<IEnumerable<HDDSmallModel>> GetSmallModelWithPagination(int page, int pageSize, CancellationToken cancellationToken);
        Task RemoveAsync(Guid id);
        Task<IEnumerable<HDDDbModel>?> SearchByName(string name, CancellationToken cancellationToken);
        Task<IEnumerable<HDDSmallModel>?> SearchByNameWithPag(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<HDDDbModel>?> GetByFilter(HDDFilterModel filterModel, CancellationToken cancellationToken);
        Task<IEnumerable<HDDSmallModel>?> GetSmallByFilter(HDDFilterModel filterModel, int page, int pageSize, CancellationToken cancellationToken);
        Task<HDDDbModel?> UpdateAsync(HDDUpdateModel updateModel);
        Task<Guid> CreateAndAttach(HDDCreateModel model, Guid PcId);
    }
}