using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Models.PSUModels.DTO;
using ByMyPC.Models.PSUModels.RDTO;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Query;

namespace ByMyPC.Services.PSUService
{
    public class PSUService(
            IPSURepo repo,
            ILogger<PSUService> logger,
            IValidator<DTOPSUModelCreate> validator,
            IMapper mapper
        ) : IPSUService
    {
        private readonly IPSURepo repo = repo;
        private readonly ILogger<PSUService> logger = logger;
        private readonly IValidator<DTOPSUModelCreate> validator = validator;
        private readonly IMapper mapper = mapper;

        #region Get
        public async Task<IEnumerable<RDTOPSUModel>> GetFullAsync(CancellationToken cancellation)
        {
            IList<RDTOPSUModel> rdto = new List<RDTOPSUModel>();
            await foreach (var item in repo.GetFullAsyncEnumerable(cancellation))
            {
                rdto.Add(Map(item));
            }
            return rdto;
        }
        public async Task<IEnumerable<RDTOPSUSmallModel>> GetSmallAsync(CancellationToken cancellation)
        {
            IList<RDTOPSUSmallModel> rdto = new List<RDTOPSUSmallModel>();
            await foreach (var item in repo.GetSmallAsyncEnumerable(cancellation))
            {
                rdto.Add(Map(item));
            }
            return rdto;

        }

        public async Task<RDTOPSUModel?> GetByID(Guid id)
        {
            var item = await repo.GetByIdAsync(id);
            return item is null ? null : Map(item);
        }

        public async Task<IEnumerable<RDTOPSUSmallModel>> SearchByName(string name, CancellationToken cancellation)
        {
            IList<RDTOPSUSmallModel> rdto = new List<RDTOPSUSmallModel>();
            await foreach (var item in repo.SearchByNameSmallAsyncEnumerable(name, cancellation))
            {
                rdto.Add(Map(item));
            }
            return rdto;
        }

        public async Task<IEnumerable<RDTOPSUSmallModel>?> GetSmallWithPag(int page, int pageSize, CancellationToken cancellation)
        {
            var data = await repo.GetSmallModelsAsyncWithPag(page, pageSize, cancellation);
            if (data is null)
                logger.LogWarning("Func {func} Return null value\nparam Page: {page}; pageSize: {pagesize}", nameof(GetSmallWithPag), page, pageSize);
            return data?.Select(Map).ToList();
        }
        #endregion

        #region Update 
        public async Task<RDTOPSUModel?> UpdateAsync(DTOPSUModelUpdate model)
        {
            PSUUpdateModel update = new(
                model.id,
                model.Name,
                model.PowerWatt,
                model.IsLive,
                model.Size,
                model.IsModular,
                model.IsCertified
                );
            var data = await repo.UpdateAsync(update);
            return data is not null ? Map(data) : null;
        }
        #endregion

        #region Create 
        public async Task<Guid> CreateAsync(DTOPSUModelCreate model)
        {
            await validator.ValidateAndThrowAsync(model);
            Guid id = await repo.CreateAsync(Map(model));
            return id;
        }
        #endregion

        #region Remove 
        public async ValueTask<bool> RemoveAsync(Guid id)
        {
            bool isDeleted = await repo.RemoveAsync(id);
            return isDeleted;
        }
        #endregion 

        #region Mappers
        private RDTOPSUModel Map(PSUDbModel model) => mapper.Map<RDTOPSUModel>(model);
        private RDTOPSUSmallModel Map(PSUSmallModel model) => mapper.Map<RDTOPSUSmallModel>(model);
        private PSUCreateModel Map(DTOPSUModelCreate model) => mapper.Map<PSUCreateModel>(model);

        #endregion
    }
}
