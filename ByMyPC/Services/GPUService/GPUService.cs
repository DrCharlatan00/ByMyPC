using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Models.GPUModels.DTO;
using ByMyPC.Models.GPUModels.RDTO;
using FluentValidation;

namespace ByMyPC.Services.GPUService
{
    public class GPUService(
        IGPURepo repo,
        IMapper mapper,
        ILogger<GPUService> logger,
        IValidator<DTOGPUCreateModel> validator)
    {
        private readonly IGPURepo repo = repo;
        private readonly IMapper mapper = mapper;
        private readonly ILogger<GPUService> logger = logger;
        private readonly IValidator<DTOGPUCreateModel> validator = validator;


        #region Get
        public async Task<IEnumerable<RDTOGPUSmallModel>> GetSmallModelsAsync(CancellationToken cancellation)
        {
            IList<RDTOGPUSmallModel> rdto = new List  <RDTOGPUSmallModel>();
            await foreach (var item in repo.GetSmallModelsDbAsync(cancellation))
            {
                rdto.Add(Map(item));
            }
            return rdto;
        }


        public async Task<IEnumerable<RDTOGPUModel>> GetFullModelsAsync(CancellationToken cancellation)
        {
            IList<RDTOGPUModel> rdto = new List<RDTOGPUModel>();
            await foreach (var item in repo.GetFullModelsDbAsync(cancellation))
            {
                rdto.Add(Map(item));
            }
            return rdto;
        }

        public async Task<RDTOGPUModel?> GetByIDAsync(Guid id )
        {
            var item = await repo.GetByID(id);
            return item == null ? null : Map(item);
        }

        public async Task<IEnumerable<RDTOGPUSmallModel>> GetSmallWithPag(int page, int pageSize, CancellationToken cancellationToken)
        {
            if (page >= 10000) throw new ArgumentException("Page is to big");
            if (pageSize >= 10000) throw new ArgumentException("Page Size is not correct");

            var data = await repo.GetSmallModelWithPagination(page, pageSize, cancellationToken);

            if (data is null) {
                logger.LogInformation("For request with param page {page} and pageSize {size} , returns data is null",page,pageSize);
                return null;
            }
            return data.Select(Map).ToList();
        }


        public async Task<IEnumerable<RDTOGPUModel>?> SearchByNameAsync(string name, CancellationToken cancellationToken)
        {
            var data = await repo.SearchByName(name, cancellationToken);
            if (data is null) {
                logger.LogInformation("For request {func} with param name: {name} , returns data is null", nameof(SearchByNameAsync), name);
                return null;
            }
            return data.Select(Map).ToList();
        }

        public async Task<IEnumerable<RDTOGPUSmallModel>?> SearchByNameAsyncWithPag(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            var data = await repo.SearchByNameWithPag(name, page, pageSize, cancellationToken);
            if (data is null)
            {
                logger.LogInformation("For request {func} with param name: {name} , returns data is null", nameof(SearchByNameAsyncWithPag), name);
                return null;
            }
            return data.Select(Map).ToList();
        }
        #endregion

        #region Mappers
        private RDTOGPUSmallModel Map(GPUSmallModel model) => mapper.Map<RDTOGPUSmallModel>(model);
        private RDTOGPUModel Map(GpuDbModel model) => mapper.Map<RDTOGPUModel>(model);

        #endregion
    }
}
