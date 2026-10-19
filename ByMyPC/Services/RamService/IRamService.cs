using ByMyPC.Models.RAMModels.DTO;
using ByMyPC.Models.RAMModels.RDTO;

namespace ByMyPC.Services.RamService
{
    public interface IRamService
    {
        ValueTask<bool> AttachAsync(Guid pcID, Guid RamID);
        ValueTask<Guid> CreateAsync(DTORAMCreate dto);
        ValueTask<bool> DeAttachAsync(Guid pcID, Guid RamID);
        Task<RDTORAMModel?> GetByIdAsync(Guid id);
        Task<IEnumerable<RDTORAMModel>> GetFullAsync(CancellationToken cancellationToken);
        Task<IEnumerable<RDTORAMModel>> GetFullByFilterAsync(DTORAMFilter filter, CancellationToken cancellationToken);
        Task<IEnumerable<RDTORAMModel>?> GetFullWithPagination(int page, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<RDTORAMSmallModel>> GetSmallAsync(CancellationToken cancellationToken);
        Task<IEnumerable<RDTORAMSmallModel>> GetSmallByFilterAsyncPaginated(DTORAMFilter filter, int page, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<RDTORAMSmallModel>?> GetSmallWithPagination(int page, int pageSize, CancellationToken cancellationToken);
        ValueTask<bool> RemoveAsync(Guid id);
        Task<IEnumerable<RDTORAMModel>?> SearchByNameAsyncFull(string name, CancellationToken cancellation);
        Task<IEnumerable<RDTORAMSmallModel>?> SearchByNameAsyncSmallPaginated(string name, int page, int pageSize, CancellationToken cancellation);
        Task<RDTORAMModel?> UpdateAsync(DTORAMUpdate update);
    }
}