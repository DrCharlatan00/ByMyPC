using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;

namespace ByMyPc.Postgresql.Repository.Intefaces
{
    public interface IPcRepo
    {
        ValueTask<bool> AttachCPUAsync(Guid idPc, Guid IdCPU);
        ValueTask<bool> AttachGPUAsync(Guid idPc, Guid IdGPU);
        ValueTask<bool> AttachMotherboardAsync(Guid idPc, Guid IdMotherboard);
        ValueTask<bool> AttachPSUAsync(Guid idPc, Guid IdPSU);
        ValueTask<Guid> CreateAsync(PcCreateModel model);
        ValueTask<bool> DeattachCPUAsync(Guid idPc, Guid idCPU);
        ValueTask<bool> DeattachGPUAsync(Guid idPc, Guid idGPU);
        ValueTask<bool> DeattachMotherbordAsync(Guid idPc, Guid idMotherboard);
        ValueTask<bool> DeattachPSUAsync(Guid idPc, Guid idPSU);
        Task<IEnumerable<PcDbModel>> GetByFilter(PcFilterModel filter, CancellationToken cancellationToken);
        Task<IEnumerable<PcSmallModel>> GetByFilterWithPagination(PcFilterModel filter, int page, int pageSize, CancellationToken cancellationToken);
        Task<PcDbModel?> GetByID(Guid id);
        IAsyncEnumerable<PcDbModel> GetFullAsync(CancellationToken cancellationToken);
        Task<IEnumerable<PcDbModel>> GetPcFullWithPagination(int page, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<PcSmallModel>> GetPcSmallWithPagination(int page, int pageSize, CancellationToken cancellationToken);
        IAsyncEnumerable<PcSmallModel> GetSmallModelsAsync(CancellationToken cancellationToken);
        ValueTask<bool> RemoveAsync(Guid id);
        Task<IEnumerable<PcDbModel>?> SearchByName(string name, CancellationToken cancellationToken);
        Task<IEnumerable<PcSmallModel>?> SearchByNameWithPag(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<PcDbModel?> UpdateAsync(PcUpdateModel model);
    }
}