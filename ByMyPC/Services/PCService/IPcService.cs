using ByMyPC.Models.PCModels.DTO;
using ByMyPC.Models.PCModels.RDTO;

namespace ByMyPC.Services.PCService
{
    public interface IPcService
    {
        ValueTask<bool> AttachAsync(Guid idPc, Guid idComponent, WithAttachOrDeattachToPc attachComponent);
        ValueTask<Guid> CreateAsync(DTOPCCreateModel createModel);
        ValueTask<bool> DeattachAsync(Guid idPc, Guid idComponent, WithAttachOrDeattachToPc attachComponent);
        Task<IEnumerable<RDTOPCModel>?> GetByFilterAsync(DTOPCFilter filter, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOPCSmallModel>?> GetByFilterAsyncPaginated(DTOPCFilter filter, int page, int pageSize, CancellationToken cancellationToken);
        Task<RDTOPCModel?> GetByIdAsync(Guid id);
        Task<IEnumerable<RDTOPCModel>> GetFullAsync(CancellationToken cancellationToken);
        Task<IEnumerable<RDTOPCModel>?> GetFullPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOPCSmallModel>> GetSmallAsync(CancellationToken cancellationToken);
        Task<IEnumerable<RDTOPCSmallModel>?> GetSmallPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken);
        ValueTask<bool> RemoveAsync(Guid id);
        Task<IEnumerable<RDTOPCModel>?> SearchByNameAsync(string name, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOPCSmallModel>?> SearchByNamePaginatedAsync(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<RDTOPCModel?> UpdateAsync(DTOPCUpdateModel updateModel);
    }
}