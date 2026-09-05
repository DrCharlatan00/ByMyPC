using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Caching;
using ByMyPC.Hubs;
using ByMyPC.Models.CpuModels.DTO;
using ByMyPC.Models.CpuModels.RDTO;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

namespace ByMyPC.Services.CpuService
{
    public class CpuService(ICpuRepo repo,
        IValidator<DTOCpuCreateModel> validatorCreate,
        IValidator<DTOCpuUpdateModel> validatorUpdate,
        IMapper mapper,
        ILogger<CpuService> logger,
        IHubContext<CpuHub> hub,
        ICacheService cacheService
        ) : ICpuService
    {
        private readonly ICpuRepo repo = repo;
        private readonly IValidator<DTOCpuCreateModel> validatorCreate = validatorCreate;
        private readonly IValidator<DTOCpuUpdateModel> validatorUpdate = validatorUpdate;
        private readonly IMapper mapper = mapper;
        private readonly ILogger<CpuService> logger = logger;
        private readonly IHubContext<CpuHub> hub = hub;
        private readonly ICacheService cacheService = cacheService;

        public async Task<IEnumerable<RDTOCpuModel>> GetFullCpuAsync(CancellationToken cancellationToken)
        {
            List<RDTOCpuModel> RDTO = new();
            await foreach (var item in repo.GetCpuRepoAsyncEnumerable(cancellationToken))
            {
                RDTO.Add(Map(item));
            }
            if (RDTO is null) logger.LogWarning("Collection {collection} is db is null", nameof(IEnumerable<RDTOCpuModel>));
            return RDTO;
        }

        public async Task<IEnumerable<RDTOCpuModel>> GetFullCpuPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            const string keyCacheV = "cpu:version";

            long vers = await cacheService.GetVersionAsync(keyCacheV);

            string key = $"cpu:v{vers}:page={page}:page_size={pageSize}";

            var cache = await cacheService.GetAsync<IEnumerable<RDTOCpuModel>>(key);

            if (cache is not null) return cache;

            var data = await repo.GetAsyncPagination(page, pageSize, cancellationToken);
            var rdto = data.Select(Map).ToList();

            await cacheService.SetAsync(key,rdto,TimeSpan.FromMinutes(2));

            return rdto;
        }

        public async Task<RDTOCpuModel?> GetById(Guid Id)
        {
            var data = await repo.GetByIDAsync(Id);
            return data is null ? null : Map(data);
        }

        public async Task<IEnumerable<RDTOCpuSmallModel>> GetRDTOSmallModelAsync(CancellationToken cancellationToken)
        {
            List<RDTOCpuSmallModel> RDTO = new();
            await foreach (var item in repo.GetCpuSmallRepoAsyncEnumerable(cancellationToken))
            {
                RDTO.Add(Map(item));
            }
            return RDTO;
        }

        public async Task<IEnumerable<RDTOCpuSmallModel>> GetSmallModelsWithPaginationAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            const string keyCacheV = "cpu:version";

            long vers = await cacheService.GetVersionAsync(keyCacheV);

            string key = $"cpu:v{vers}:page={page}:page_size={pageSize}";

            var cache = await cacheService.GetAsync<IEnumerable<RDTOCpuSmallModel>>(key);

            if (cache is not null) return cache;

            var data = await repo.GetCpuSmallModelsPagination(page, pageSize, cancellationToken);
            var rdto = data.Select(Map).ToList();

            await cacheService.SetAsync(key, rdto, TimeSpan.FromMinutes(2));

            return rdto;
        }

        public async Task<IEnumerable<RDTOCpuSmallModel>> SearchByNameAsync(string name, CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("Func {func} Get : name: {name}", nameof(SearchByNameAsync), name);
#endif


            List<RDTOCpuSmallModel> RDTO = new();
            await foreach (var data in repo.SearchCpuSmallByNameAsyncEnumerable(name, cancellationToken))
            {
                RDTO.Add(Map(data));
            }
            return RDTO;

        }

        public async Task<IEnumerable<RDTOCpuModel>?> GetByFilterAsync(DTOCpuFilter filter,CancellationToken cancellationToken)
        {
            var data = await repo.GetByFilterAsync(filter.ConvertToDbFilter(filter),cancellationToken);
            return data is not null ? data.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOCpuSmallModel>?> GetByFilterWithPagAsync(DTOCpuFilter filter, int page, int pageSize, CancellationToken cancellationToken)
        {
            var data = await repo.GetByFilterWithPagAsync(filter.ConvertToDbFilter(filter),page,pageSize, cancellationToken);
            return data is not null ? data.Select(MapToFull).ToList() : null;
        }

        public async Task<IEnumerable<RDTOCpuSmallModel>> SearchByNameWithPaginationAsync(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("Func {func} Get : name: {name} , page: {page} , pageSize: {pageSize}", nameof(SearchByNameWithPaginationAsync), name, page, pageSize);
#endif

            const string keyCacheV = "cpu:version";

            long vers = await cacheService.GetVersionAsync(keyCacheV);

            string key = $"cpu:v{vers}:name={name}:page={page}:page_size={pageSize}";

            var cache = await cacheService.GetAsync<IEnumerable<RDTOCpuSmallModel>>(key);

            if (cache is not null) return cache;
            List<RDTOCpuSmallModel> RDTO = new();
            await foreach (var data in repo.SearchCpuSmallByNameWithPaginationAsyncEnumerable(name, page, pageSize, cancellationToken))
            {
                RDTO.Add(Map(data));
            }

            await cacheService.SetAsync(key, RDTO, TimeSpan.FromMinutes(2));

            return RDTO;
        }

        public async Task<RDTOCpuModel?> UpdateAsync(DTOCpuUpdateModel model)
        {
#if DEBUG
            logger.LogInformation("Func {func} Get : model: {@model}", nameof(UpdateAsync), model);
#endif
            ArgumentNullException.ThrowIfNull(model);
            await validatorUpdate.ValidateAndThrowAsync(model);
            var result = await repo.UpdateAsync(model.id, Map(model));
            if (result is null) return null;
            await hub.Clients.All.SendAsync("CpuUpdated", result.ID);
            
            const string keyCacheV = "cpu:version";
            await cacheService.IncrementAsync(keyCacheV);
            
            return Map(result);

        }

        public async Task<Guid> CreateAsync(DTOCpuCreateModel model)
        {
#if DEBUG
            logger.LogInformation("Func {func} Get : model: {@model}", nameof(CreateAsync), model);
#endif
            ArgumentNullException.ThrowIfNull(model);
            await validatorCreate.ValidateAndThrowAsync(model);
            var result = await repo.CreateAsync(Map(model));
            await hub.Clients.All.SendAsync("NewCpuCreated", result);

            const string keyCacheV = "cpu:version";
            await cacheService.IncrementAsync(keyCacheV);

            return result;
        }

        public async Task RemoveAsync(Guid id)
        {
#if DEBUG
            logger.LogInformation("Func {func} Get : id: {id}", nameof(RemoveAsync), id);
#endif
            await repo.RemoveAsync(id);
            await hub.Clients.All.SendAsync("CpuRemoved", id);

            const string keyCacheV = "cpu:version";
            await cacheService.IncrementAsync(keyCacheV);

        }


        private RDTOCpuModel Map(CpuDbModel model) => mapper.Map<RDTOCpuModel>(model);
        private RDTOCpuSmallModel MapToFull(CpuDbModel model) => mapper.Map<RDTOCpuSmallModel>(model);

        private RDTOCpuSmallModel Map(CpuSmallModel model) => mapper.Map<RDTOCpuSmallModel>(model);
        private CpuUpdateModel Map(DTOCpuUpdateModel model) => mapper.Map<CpuUpdateModel>(model);
        private CpuCreateModel Map(DTOCpuCreateModel model) => mapper.Map<CpuCreateModel>(model);

    }


}
