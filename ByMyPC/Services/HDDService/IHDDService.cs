using ByMyPC.Models.HDDModels.DTO;
using ByMyPC.Models.HDDModels.RDTO;

namespace ByMyPC.Services.HDDService
{
    public interface IHDDService
    {
        ValueTask<bool> AttachHdd(Guid pcId, Guid HddId);
        Task<Guid> CreateAsync(DTOHDDCreateModel dto);
        ValueTask<bool> DeattachHdd(Guid pcId, Guid HddId);
        Task<IEnumerable<RDTOHDDModel>?> GetByFilter(DTOHDDFilter filter, CancellationToken cancellationToken);
        Task<RDTOHDDModel?> GetByIdAsync(Guid id);
        Task<IEnumerable<RDTOHDDCardModel>?> GetCardByFilter(DTOHDDFilter filter, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOHDDCardModel>> GetCardModelsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<RDTOHDDCardModel>> GetCardWithPagination(int page, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOHDDModel>> GetModelsAsync(CancellationToken cancellationToken);
        Task RemoveAsync(Guid id);
        Task<IEnumerable<RDTOHDDModel>?> SearchByName(string name, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOHDDCardModel>?> SearchByName(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<RDTOHDDModel?> UpdateAsync(DTOHDDUpdateModel model);
    }
}