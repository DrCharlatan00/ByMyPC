using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Hubs;
using ByMyPC.Models.CpuModels.RDTO;
using ByMyPC.Models.MotherbordModels.DTO;
using ByMyPC.Models.MotherbordModels.RDTO;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;

namespace ByMyPC.Services.MotherboardService
{
    public class MotherboardService(
        IMotherboardRepo repo,
        IValidator<DTOMotherboardCreateModel> validatorCreate,
        IValidator<DTOMotherboardUpdateModel> validatorUpdate,
        IMapper mapper,
        ILogger<MotherboardService> logger,
        IHubContext<MotherboardHub> hub
        ) : IMotherboardService
    {
        private readonly IMotherboardRepo repo = repo;
        private readonly IValidator<DTOMotherboardCreateModel> validatorCreate = validatorCreate;
        private readonly IValidator<DTOMotherboardUpdateModel> validatorUpdate = validatorUpdate;
        private readonly IMapper mapper = mapper;
        private readonly ILogger<MotherboardService> logger = logger;
        private readonly IHubContext<MotherboardHub> hub = hub;

        #region Get
        public async Task<IEnumerable<RDTOModelMotherboardCard>> GetCardMotherboardAsync(CancellationToken cancellationToken)
        {
            ICollection<RDTOModelMotherboardCard> rdto = new List<RDTOModelMotherboardCard>();
            await foreach (var item in repo.GetCardMotherboardDbAsync(cancellationToken))
            {
                rdto.Add(Map(item));
            }
            return rdto;
        }

        public async Task<IEnumerable<RDTOModelMotherboard>> GetFullMotherboardAsync(CancellationToken cancellationToken)
        {
            ICollection<RDTOModelMotherboard> rdto = new List<RDTOModelMotherboard>();
            await foreach (var item in repo.GetFullMotherboardDbAsync(cancellationToken))
            {
                rdto.Add(Map(item));
            }
            return rdto;
        }

        public async Task<RDTOModelMotherboard?> GetByIdAsync(Guid id)
        {
            MotherboardDbModel? data = await repo.GetByIDAsync(id);
            return data != null ? Map(data) : null;
        }

        public async Task<IEnumerable<RDTOModelMotherboardCard>?> GetCardWithPaginationAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var data = await repo.GetCardWithPaginationAsync(page, pageSize, cancellationToken);
            return data != null ? data.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOModelMotherboardCard>?> SearchByNameAsync(string name, CancellationToken cancellationToken)
        {
            var data = await repo.SearchByNameMotherboardSmallAsync(name, cancellationToken);
            return data != null ? data.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOModelMotherboardCard>?> SearchByNameWithPaginationAsync(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            var data = await repo.SearchByNameMotherboardSmallWithPaginationAsync(name, page, pageSize, cancellationToken);
            return data != null ? data.Select(Map).ToList() : null;
        }
        #endregion

        #region Update
        public async Task<RDTOModelMotherboard?> UpdateAsync(DTOMotherboardUpdateModel model)
        {
            await validatorUpdate.ValidateAndThrowAsync(model);
            MotherboardDbModel? res = await repo.UpdateAsync(Map(model));
            if (res is null)
            {
                logger.LogInformation("Func {func} not update Motherboard with id: {id} \nModel: {@model}", nameof(UpdateAsync), model.id, model);
                return null;
            }
            await hub.Clients.All.SendAsync("MotherboardUpdated",res.ID);
            return Map(res);
        }
        #endregion

        #region Create
        public async Task<Guid> CreateAsync(DTOMotherboardCreateModel model)
        {
            try
            {
                await validatorCreate.ValidateAndThrowAsync(model);
                Guid id = await repo.CreateAsync(Map(model));
                await hub.Clients.All.SendAsync("NewMotherboardCreated", id);
                return id;
            }
            catch (Exception ex) {
                logger.LogWarning(ex, "Item not created model: {@model}", model);
                throw;
            }
        }
        #endregion

        #region Remove
        public async Task RemoveAsync(Guid id)
        {
            await repo.RemoveAsync(id);
            await hub.Clients.All.SendAsync("MotherboardRemoved",id);
        }
        #endregion

        #region Mapping
        private RDTOModelMotherboardCard Map(MotherboardSmallDbModel model) => mapper.Map<RDTOModelMotherboardCard>(model);
        private RDTOModelMotherboard Map(MotherboardDbModel model) => mapper.Map<RDTOModelMotherboard>(model);
        private MotherboardUpdateModel Map(DTOMotherboardUpdateModel model) => mapper.Map<MotherboardUpdateModel>(model);
        private MotherboardCreateModel Map(DTOMotherboardCreateModel model) => mapper.Map<MotherboardCreateModel>(model);
        #endregion
    }
}
