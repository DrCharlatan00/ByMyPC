using ByMyPC.Models.PSUModels;
using ByMyPC.Models.PSUModels.DTO;
using ByMyPC.Models.PSUModels.RDTO;

namespace ByMyPC.Services.PSUService
{
    public interface IPSUService
    {
        Task<Guid> CreateAsync(DTOPSUModelCreate model);
        Task<IEnumerable<RDTOPSUModel>> GetByFilterFull(DTOPSUFilterModel model, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOPSUSmallModel>> GetByFilterSmall(DTOPSUFilterModel model, CancellationToken cancellationToken);
        Task<RDTOPSUModel?> GetByID(Guid id);
        Task<IEnumerable<RDTOPSUModel>> GetFullAsync(CancellationToken cancellation);
        Task<IEnumerable<RDTOPSUModel>?> GetFullWithPag(int page, int pageSize, CancellationToken cancellation);
        Task<IEnumerable<RDTOPSUSmallModel>> GetSmallAsync(CancellationToken cancellation);
        Task<IEnumerable<RDTOPSUSmallModel>?> GetSmallWithPag(int page, int pageSize, CancellationToken cancellation);
        ValueTask<bool> RemoveAsync(Guid id);
        Task<IEnumerable<RDTOPSUSmallModel>> SearchByName(string name, CancellationToken cancellation);
        Task<IEnumerable<RDTOPSUModel>> SearchByNameFullAsync(string name, CancellationToken cancellation);
        Task<RDTOPSUModel?> UpdateAsync(DTOPSUModelUpdate model);
    }
}