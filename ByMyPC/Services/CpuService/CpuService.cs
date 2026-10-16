using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Caching;
using ByMyPC.Hubs;
using ByMyPC.Models.CpuModels.DTO;
using ByMyPC.Models.CpuModels.RDTO;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
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

        public async Task<IEnumerable<RDTOCpuModel>?> GetFullCpuAsync(CancellationToken cancellationToken)
        {

#if DEBUG
            logger.LogInformation("---------------------Start Work Service--------------------");
#endif

            List<RDTOCpuModel> RDTO = new();

#if DEBUG
            logger.LogInformation("Start Db Proccess......");
#endif


            await foreach (var item in repo.GetCpuRepoAsyncEnumerable(cancellationToken))
            {
                RDTO.Add(Map(item));
            }

#if DEBUG
            logger.LogInformation("End Db Procces, next check data null");
#endif


            if (RDTO is null) logger.LogWarning("Collection {collection} is db is null", nameof(IEnumerable<RDTOCpuModel>));
#if DEBUG
            logger.LogInformation("---------------------End Work Service--------------------");
#endif

#if DEBUG
            logger.LogInformation("Return data");
#endif


            return RDTO;
        }

        public async Task<IEnumerable<RDTOCpuModel>> GetFullCpuPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("---------------------Start Work Service--------------------");
#endif

            const string keyCacheV = "cpu:version";

            long vers = await cacheService.GetVersionAsync(keyCacheV);

            string key = $"cpu:v{vers}:page={page}:page_size={pageSize}";

#if DEBUG
            logger.LogInformation("Try Get Cache data.......");
#endif


            var cache = await cacheService.GetAsync<IEnumerable<RDTOCpuModel>>(key);

            if (cache is not null) return cache;

#if DEBUG
            logger.LogInformation("Cache missing, continue db proccess.........");
#endif


            var data = await repo.GetAsyncPagination(page, pageSize, cancellationToken);

#if DEBUG
            logger.LogInformation("End db Get procces, Mapping....");
#endif


            var rdto = data.Select(Map).ToList();


#if DEBUG
            logger.LogInformation("Map end, Set Cache.........");
#endif


            await cacheService.SetAsync(key,rdto,TimeSpan.FromMinutes(2));
#if DEBUG
            logger.LogInformation("---------------------End Work Service--------------------");
#endif
            return rdto;
        }

        public async Task<RDTOCpuModel?> GetById(Guid Id)
        {
#if DEBUG
            logger.LogInformation("---------------------Start Work Service--------------------");
#endif

#if DEBUG
            logger.LogInformation("GetByID With ID = {id}",Id);
#endif

#if DEBUG
            logger.LogInformation("Start db procces........");
#endif



            var data = await repo.GetByIDAsync(Id);

#if DEBUG
            logger.LogInformation("End Db procces, Result = {res}",data);
#endif

#if DEBUG
            logger.LogInformation("---------------------End Work Service--------------------");
#endif
            return data is null ? null : Map(data);
        }

        public async Task<IEnumerable<RDTOCpuSmallModel>> GetRDTOSmallModelAsync(CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("---------------------Start Work Service--------------------");
#endif
            List<RDTOCpuSmallModel> RDTO = new();

#if DEBUG
            logger.LogInformation("Start DB Async Enumerable Get and Mapping...........");
#endif


            await foreach (var item in repo.GetCpuSmallRepoAsyncEnumerable(cancellationToken))
            {
                RDTO.Add(Map(item));
            }

#if DEBUG
            logger.LogInformation("End Procces Get, NULL? = {res}", RDTO is null ? true : false);
#endif



#if DEBUG
            logger.LogInformation("---------------------End Work Service--------------------");
#endif
            return RDTO;
        }

        public async Task<IEnumerable<RDTOCpuSmallModel>> GetSmallModelsWithPaginationAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("---------------------Start Work Service--------------------");
#endif
            CheckParam(page, pageSize);


#if DEBUG
            logger.LogInformation("CheckParam pass, param page = {page} pageSize = {pageSize}",page,pageSize);
#endif


            const string keyCacheV = "cpu:version";

            long vers = await cacheService.GetVersionAsync(keyCacheV);

            string key = $"cpu:v{vers}:page={page}:page_size={pageSize}";

#if DEBUG
            logger.LogInformation("Try Get in Cache..........");
#endif


            var cache = await cacheService.GetAsync<IEnumerable<RDTOCpuSmallModel>>(key);

            if (cache is not null) return cache;



#if DEBUG
            logger.LogInformation("Cache is miss, get in db..............");
#endif


            var data = await repo.GetCpuSmallModelsPagination(page, pageSize, cancellationToken);


#if DEBUG
            logger.LogInformation("Db return result, Mapping...........");
#endif


            var rdto = data.Select(Map).ToList();

#if DEBUG
            logger.LogInformation("Map end, Caching.............");
#endif


            await cacheService.SetAsync(key, rdto, TimeSpan.FromMinutes(2));

#if DEBUG
            logger.LogInformation("Caching end, return data ");
#endif


#if DEBUG
            logger.LogInformation("---------------------End Work Service--------------------");
#endif
            return rdto;
        }

        public async Task<IEnumerable<RDTOCpuSmallModel>> SearchByNameAsync(string name, CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("---------------------Start Work Service--------------------");
#endif
#if DEBUG
            logger.LogInformation("Func {func} Get : name: {name}", nameof(SearchByNameAsync), name);
#endif
            CheckParam(name);

#if DEBUG
            logger.LogInformation("CheckParam pass, param  name = {name}",name);
#endif



            List<RDTOCpuSmallModel> RDTO = new();
#if DEBUG
            logger.LogInformation("Start DB Async Enumerable Get and Mapping...........");
#endif

            await foreach (var data in repo.SearchCpuSmallByNameAsyncEnumerable(name, cancellationToken))
            {
                RDTO.Add(Map(data));
            }

#if DEBUG
            logger.LogInformation("End Get Data, return");
#endif


#if DEBUG
            logger.LogInformation("---------------------End Work Service--------------------");
#endif
            return RDTO;

        }

        public async Task<IEnumerable<RDTOCpuModel>?> GetByFilterAsync(DTOCpuFilter filter,CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("---------------------Start Work Service--------------------");
#endif

#if DEBUG
            logger.LogInformation("Start db procces, with next data = {@filter}", filter);
#endif


            var data = await repo.GetByFilterAsync(Map(filter),cancellationToken);

#if DEBUG
            logger.LogInformation("Procces db end, Mapping and return data, result operation = {}", data is null ? false : true);
#endif


#if DEBUG
            logger.LogInformation("---------------------End Work Service--------------------");
#endif
            return data is not null ? data.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOCpuSmallModel>?> GetByFilterWithPagAsync(DTOCpuFilter filter, int page, int pageSize, CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("---------------------Start Work Service--------------------");
#endif
            CheckParam(page, pageSize);

#if DEBUG
            logger.LogInformation("CheckParam pass, param page = {page}, PageSize = {pageSize}",page, pageSize);
#endif


            logger.LogInformation("Start db procces, with next data = {@filter}", filter);

            var data = await repo.GetByFilterWithPagAsync(Map(filter),page,pageSize, cancellationToken);
#if DEBUG
            logger.LogInformation("Procces db end, Mapping and return data, result operation = {}", data is null ? false : true);
#endif

#if DEBUG
            logger.LogInformation("---------------------End Work Service--------------------");
#endif
            return data is not null ? data.Select(MapToFull).ToList() : null;
        }

        public async Task<IEnumerable<RDTOCpuSmallModel>> SearchByNameWithPaginationAsync(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("---------------------Start Work Service--------------------");
#endif
#if DEBUG
            logger.LogInformation("Func {func} Get : name: {name} , page: {page} , pageSize: {pageSize}", nameof(SearchByNameWithPaginationAsync), name, page, pageSize);
#endif
            CheckParam(page, pageSize, name);

#if DEBUG
            logger.LogInformation("CheckParam pass, param name = {name}, page = {page} , pageSize = {pageSize}",name,page,pageSize);
#endif


            const string keyCacheV = "cpu:version";

            long vers = await cacheService.GetVersionAsync(keyCacheV);

            string key = $"cpu:v{vers}:name={name}:page={page}:page_size={pageSize}";

#if DEBUG
            logger.LogInformation("Try get data in Cache..........");
#endif


            var cache = await cacheService.GetAsync<IEnumerable<RDTOCpuSmallModel>>(key);

            if (cache is not null) return cache;


#if DEBUG
            logger.LogInformation("Cache is miss, get in db Async Enumerable........");
#endif


            List<RDTOCpuSmallModel> RDTO = new();
            await foreach (var data in repo.SearchCpuSmallByNameWithPaginationAsyncEnumerable(name, page, pageSize, cancellationToken))
            {
                RDTO.Add(Map(data));
            }


#if DEBUG
            logger.LogInformation("End db procces, Caching data.........");
#endif



            await cacheService.SetAsync(key, RDTO, TimeSpan.FromMinutes(2));
#if DEBUG
            logger.LogInformation("---------------------End Work Service--------------------");
#endif
            return RDTO;
        }

        public async Task<RDTOCpuModel?> UpdateAsync(DTOCpuUpdateModel model)
        {
#if DEBUG
            logger.LogInformation("---------------------Start Work Service--------------------");
#endif
#if DEBUG
            logger.LogInformation("Func {func} Get : model: {@model}", nameof(UpdateAsync), model);
#endif

#if DEBUG
            logger.LogInformation("Check null and validate model = {@model}",model);
#endif


            ArgumentNullException.ThrowIfNull(model);
            await validatorUpdate.ValidateAndThrowAsync(model);

#if DEBUG
            logger.LogInformation("Model pass, Start Db Updating............");
#endif


            var result = await repo.UpdateAsync(model.id, Map(model));

#if DEBUG
            logger.LogInformation("Db Update end");
#endif


            if (result is null) return null;

#if DEBUG
            logger.LogInformation("Operation in db OK, Start Side services..........");
#endif


            var signalR =  hub.Clients.All.SendAsync("CpuUpdated", result.ID);       
            const string keyCacheV = "cpu:version";
            var incrementCache = cacheService.IncrementAsync(keyCacheV);
            try {
                await Task.WhenAll(incrementCache, signalR);
            }
            catch(Exception ex)
            {
                logger.LogError(ex,"Redis or SignalR error\n ex message: {mes}",ex.Message);
            }
#if DEBUG
            logger.LogInformation("---------------------End Work Service--------------------");
#endif
            return Map(result);

        }

        public async ValueTask<Guid> CreateAsync(DTOCpuCreateModel model)
        {
#if DEBUG
            logger.LogInformation("---------------------Start Work Service--------------------");
#endif
#if DEBUG
            logger.LogInformation("Func {func} Get : model: {@model}", nameof(CreateAsync), model);
#endif
#if DEBUG
            logger.LogInformation("Check null and validate model = {@model}", model);
#endif
            ArgumentNullException.ThrowIfNull(model);
            await validatorCreate.ValidateAndThrowAsync(model);

#if DEBUG
            logger.LogInformation("Model pass, Start Db Creating............");
#endif
            var result = await repo.CreateAsync(Map(model));

#if DEBUG
            logger.LogInformation("Db end Operation, CPU Created\nStart Side Services.....................");
#endif


            var signalR = hub.Clients.All.SendAsync("NewCpuCreated", result);

            const string keyCacheV = "cpu:version";
            var incrementCache = cacheService.IncrementAsync(keyCacheV);
            try
            {
                await Task.WhenAll(incrementCache, signalR);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Redis or SignalR error\n ex message: {mes}", ex.Message);
            }
#if DEBUG
            logger.LogInformation("---------------------End Work Service--------------------");
#endif
            return result;
        }

        public async ValueTask<bool> RemoveAsync(Guid id)
        {
#if DEBUG
            logger.LogInformation("---------------------Start Work Service--------------------");
#endif
#if DEBUG
            logger.LogInformation("Func {func} Get : id: {id}", nameof(RemoveAsync), id);
#endif

#if DEBUG
            logger.LogInformation("Start Db Removing.........");
#endif


            bool result = await repo.RemoveAsync(id);

#if DEBUG
            logger.LogInformation("End Db Procces");
#endif


            if (result == false) return false;

#if DEBUG
            logger.LogInformation("Item Removed, Start Side Services..........");
#endif


            var signalR = hub.Clients.All.SendAsync("CpuRemoved", id);

            const string keyCacheV = "cpu:version";
            var incrementCache = cacheService.IncrementAsync(keyCacheV);
            try
            {
                await Task.WhenAll(incrementCache, signalR);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Redis or SignalR error\n ex message: {mes}", ex.Message);
            }
            
#if DEBUG
            logger.LogInformation("---------------------End Work Service--------------------");
#endif
            return true;


        }


        private RDTOCpuModel Map(CpuDbModel model) => mapper.Map<RDTOCpuModel>(model);
        private RDTOCpuSmallModel MapToFull(CpuDbModel model) => mapper.Map<RDTOCpuSmallModel>(model);
        private CPUFilterModel Map(DTOCpuFilter model) => new CPUFilterModel(
            model.ByName,
            model.ByLive,
            model.ByQuantityCores
            );
        private RDTOCpuSmallModel Map(CpuSmallModel model) => mapper.Map<RDTOCpuSmallModel>(model);
        private CpuUpdateModel Map(DTOCpuUpdateModel model) => mapper.Map<CpuUpdateModel>(model);
        private CpuCreateModel Map(DTOCpuCreateModel model) => mapper.Map<CpuCreateModel>(model);

        #region Checkers
        private static void CheckParam(int page, int pageSize)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(page);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);
            if (page > 10000) throw new ArgumentException("Page is high!");
            if (pageSize > 10000) throw new ArgumentException("Page Size is to long!");
        }

        private static void CheckParam(int page, int pageSize, string name)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(page);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);
            if (page > 10000) throw new ArgumentException("Page is high!");
            if (pageSize > 10000) throw new ArgumentException("Page Size is to long!");

            ArgumentException.ThrowIfNullOrWhiteSpace(name);
        }

        private static void CheckParam(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
        }
        #endregion

    }


}
