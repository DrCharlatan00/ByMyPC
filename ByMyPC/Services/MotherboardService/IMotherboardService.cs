using ByMyPC.Models.MotherbordModels.DTO;
using ByMyPC.Models.MotherbordModels.RDTO;

namespace ByMyPC.Services.MotherboardService
{
    public interface IMotherboardService
    {
        Task<Guid> CreateAsync(DTOMotherboardCreateModel model);
        Task<RDTOModelMotherboard?> GetByIdAsync(Guid id);
        Task<IEnumerable<RDTOModelMotherboardCard>> GetCardMotherboardAsync(CancellationToken cancellationToken);
        Task<IEnumerable<RDTOModelMotherboardCard>?> GetCardWithPaginationAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOModelMotherboard>> GetFullMotherboardAsync(CancellationToken cancellationToken);
        Task RemoveAsync(Guid id);
        Task<IEnumerable<RDTOModelMotherboardCard>?> SearchByNameAsync(string name, CancellationToken cancellationToken);
        Task<IEnumerable<RDTOModelMotherboardCard>?> SearchByNameWithPaginationAsync(string name, int page, int pageSize, CancellationToken cancellationToken);
        Task<RDTOModelMotherboard?> UpdateAsync(DTOMotherboardUpdateModel model);
    }
}