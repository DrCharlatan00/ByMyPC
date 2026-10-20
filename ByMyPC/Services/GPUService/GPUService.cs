using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Caching;
using ByMyPC.Hubs;
using ByMyPC.Models.GPUModels.DTO;
using ByMyPC.Models.GPUModels.RDTO;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Abstractions;
using System.Runtime.InteropServices.Marshalling;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ByMyPC.Services.GPUService
{
    public class GPUService(
        IGPURepo repo,
        IMapper mapper,
        ILogger<GPUService> logger,
        IValidator<DTOGPUCreateModel> validator,
        ICacheService cache,
        IHubContext<GPUHub> hub
        ) : IGPUService
    {
        private readonly IGPURepo repo = repo;
        private readonly IMapper mapper = mapper;
        private readonly ILogger<GPUService> logger = logger;
        private readonly IValidator<DTOGPUCreateModel> validator = validator;
        private readonly ICacheService cache = cache;
        private readonly IHubContext<GPUHub> hub = hub;
        private const string CacheKeyVersion = "gpu:version";


        #region Get
        public async Task<IEnumerable<RDTOGPUSmallModel>> GetSmallModelsAsync(CancellationToken cancellation)
        {
            IList<RDTOGPUSmallModel> rdto = new List<RDTOGPUSmallModel>();

#if DEBUG
            logger.LogInformation("----------------------------------------------------Start Service Work----------------------------------------------------");
#endif


#if DEBUG
            logger.LogInformation("Start db get AsyncEnumerate");
#endif


            await foreach (var item in repo.GetSmallModelsDbAsync(cancellation))
            {
                rdto.Add(Map(item));
            }
            
            return rdto;
        }


        public async Task<IEnumerable<RDTOGPUModel>> GetFullModelsAsync(CancellationToken cancellation)
        {
#if DEBUG
            logger.LogInformation("----------------------------------------------------Start Service Work----------------------------------------------------");
#endif

            IList<RDTOGPUModel> rdto = new List<RDTOGPUModel>();

#if DEBUG
            logger.LogInformation("Start db get AsyncEnumerate");
#endif
            await foreach (var item in repo.GetFullModelsDbAsync(cancellation))
            {
                rdto.Add(Map(item));
            }
            return rdto;
        }

        public async Task<IEnumerable<RDTOGPUModel>?> GetFullWithPag(int page, int pageSize, CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("----------------------------------------------------Start Service Work----------------------------------------------------");
#endif

            CheckParam(page, pageSize);

            if (page > 10000) throw new ArgumentException("Page is high!");
            if (pageSize > 10000) throw new ArgumentException("Page Size is to long!");

            var data = await repo.GetFullWithPag(page, pageSize, cancellationToken);
            if (data is null)
            {
                logger.LogWarning("With func {func} data return null", nameof(GetFullWithPag));
                return null;
            }
            return [.. data.Select(Map)];
        }

        public async Task<RDTOGPUModel?> GetByIDAsync(Guid id)
        {
#if DEBUG
            logger.LogInformation("----------------------------------------------------Start Service Work----------------------------------------------------");
#endif

#if DEBUG
            logger.LogInformation("Get By ID = {id}",id);
#endif


#if DEBUG
            logger.LogInformation("Start Db Proccess......");
#endif


            var item = await repo.GetByID(id);


#if DEBUG
            logger.LogInformation("End Db Proccesss, result = {res}",item);
#endif


            return item == null ? null : Map(item);
        }

        public async Task<IEnumerable<RDTOGPUSmallModel>?> GetSmallWithPag(int page, int pageSize, CancellationToken cancellationToken)
        {
            CheckParam(page, pageSize);

#if DEBUG
            logger.LogInformation("CheckParm pass, param  page = {page}, pageSize = {pageSize}",page,pageSize);
#endif


            var keyVer = await cache.GetVersionAsync(CacheKeyVersion);

            string CacheKey = $"gpu:v{keyVer}:page={page}:pageSize={pageSize}";

#if DEBUG
            logger.LogInformation("Try Get In Cache.........");
#endif



            var cacheData = await cache.GetAsync<IEnumerable<RDTOGPUSmallModel>>(CacheKey);

            if (cacheData is not null) return cacheData;

#if DEBUG
            logger.LogInformation("Cache in miss, Continue in DB ........");
#endif



            var data = await repo.GetSmallModelWithPagination(page, pageSize, cancellationToken);


#if DEBUG
            logger.LogInformation("Db Procces end");
#endif


            if (data is null)
            {
                logger.LogInformation("For request with param page {page} and pageSize {size} , returns data is null", page, pageSize);
                return null;
            }

#if DEBUG
            logger.LogInformation("Mapping data.........");
#endif


            var dataR = data.Select(Map).ToList();

#if DEBUG
            logger.LogInformation("Caching..........");
#endif


            await cache.SetAsync(CacheKey,dataR,TimeSpan.FromMinutes(3));

#if DEBUG
            logger.LogInformation("Return data");
#endif


            return dataR;
        }


        public async Task<IEnumerable<RDTOGPUModel>?> SearchByNameAsync(string name, CancellationToken cancellationToken)
        {
            CheckParam(name);

#if DEBUG
            logger.LogInformation("Check param pass, param name = {name}",name);
#endif

#if DEBUG
            logger.LogInformation("Start Search in DB...............");
#endif


            var data = await repo.SearchByName(name, cancellationToken);

#if DEBUG
            logger.LogInformation("End Db Procces");
#endif


            if (data is null)
            {
                logger.LogInformation("For request {func} with param name: {name} , returns data is null", nameof(SearchByNameAsync), name);
                return null;
            }

#if DEBUG
            logger.LogInformation("Mapping and return");
#endif


            return data.Select(Map).ToList();
        }

        public async Task<IEnumerable<RDTOGPUSmallModel>?> SearchByNameAsyncWithPag(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            CheckParam(page, pageSize, name);

#if DEBUG
            logger.LogInformation("Check param pass, param page = {page} pageSize = {pageSize} name = {name}", page, pageSize, name);
#endif


            var keyVer = await cache.GetVersionAsync(CacheKeyVersion);

            string CacheKey = $"gpu:v{keyVer}:name={name}:page={page}:pageSize={pageSize}";

            var cacheData = await cache.GetAsync<IEnumerable<RDTOGPUSmallModel>>(CacheKey);

#if DEBUG
            logger.LogInformation("Try Get Cache...................");
#endif


            if (cacheData is not null) return cacheData;


#if DEBUG
            logger.LogInformation("Cache is miss, continue in DB......................");
#endif



            var data = await repo.SearchByNameWithPag(name, page, pageSize, cancellationToken);

#if DEBUG
            logger.LogInformation("Db end Procces");
#endif


            if (data is null)
            {
                logger.LogInformation("For request {func} with param name: {name} , returns data is null", nameof(SearchByNameAsyncWithPag), name);
                return null;
            }


#if DEBUG
            logger.LogInformation("Mapping data...............");
#endif


            var dataR = data.Select(Map).ToList();

#if DEBUG
            logger.LogInformation("Caching.............");
#endif


            await cache.SetAsync(CacheKey, dataR, TimeSpan.FromMinutes(3));

#if DEBUG
            logger.LogInformation("Return data");
#endif


            return dataR;
        }

        public async Task<IEnumerable<RDTOGPUModel>> GetByFilterAsync(DTOGPUFilter filter, CancellationToken cancellationToken)
        {

#if DEBUG
            logger.LogInformation("Start Db Filtering..................");
#endif


            var data = await repo.GetByFilter(Map(filter), cancellationToken);

#if DEBUG
            logger.LogInformation("End db Filtering, Mapping and return");
#endif


            return data.Select(Map).ToList();
        }

        public async Task<IEnumerable<RDTOGPUSmallModel>> GetByFilterWithPagAsync(DTOGPUFilter filter, int page, int pageSize, CancellationToken cancellationToken)
        {
            var keyVer = await cache.GetVersionAsync(CacheKeyVersion);

            string CacheKey = $"gpu:v{keyVer}:filter{filter}:page={page}:pageSize={pageSize}";

#if DEBUG
            logger.LogInformation("Try Get in Cache..............");
#endif


            var cacheData = await cache.GetAsync<IEnumerable<RDTOGPUSmallModel>>(CacheKey);

            if (cacheData is not null) return cacheData;


#if DEBUG
            logger.LogInformation("Cache is miss, continue in db.................");
#endif


            IEnumerable<GPUSmallModel> data = await repo.GetSmallByFilter(Map(filter), page, pageSize, cancellationToken);

#if DEBUG
            logger.LogInformation("DB End Procces");
#endif

#if DEBUG
            logger.LogInformation("Mapping..............");
#endif



            var dataR = data.Select(Map).ToList();


#if DEBUG
            logger.LogInformation("Data mapped,Caching..............");
#endif

            await cache.SetAsync(CacheKey, dataR, TimeSpan.FromMinutes(3));

#if DEBUG
            logger.LogInformation("Data Cached, return");
#endif


            return dataR;
        }
        #endregion

        #region Update
        public async Task<RDTOGPUModel?> UpdateAsync(DTOGPUUpdateModel model)
        {
            if (model.ID == Guid.Empty) throw new ArgumentException("You cannot update an empty ID.");


#if DEBUG
            logger.LogInformation("Start Updating in DB..............");
#endif


            var data = await repo.UpdateAsync(Map(model));

#if DEBUG
            logger.LogInformation("End Db updating");
#endif


            if (data is null)
            {

#if DEBUG
                logger.LogInformation("Item not found, Updade failed");
#endif

                return null;
            }
            else
            {

#if DEBUG
                logger.LogInformation("Item Updated, Start Side Services........");
#endif


                var caching = cache.IncrementAsync(CacheKeyVersion);
                var signal = hub.Clients.All.SendAsync("GPUUpdated", data.ID);

                await Task.WhenAll(caching, signal);

#if DEBUG
                logger.LogInformation("Mapping and return");
#endif


                return Map(data);
            }
        }
        #endregion

        #region Create
        public async ValueTask<Guid> CreateAsync(DTOGPUCreateModel model)
        {

#if DEBUG
            logger.LogInformation("Validating............");
#endif


            await validator.ValidateAndThrowAsync(model);


#if DEBUG
            logger.LogInformation("Validator pass data, start db Creating............");
#endif


            var data = await repo.CreateAsync(Map(model));

#if DEBUG
            logger.LogInformation("Db end procces, Start Side Service..............");
#endif


            var caching = cache.IncrementAsync(CacheKeyVersion);
            var signal = hub.Clients.All.SendAsync("GPUCreated",data);

            await Task.WhenAll(caching,signal);

#if DEBUG
            logger.LogInformation("Return data");
#endif


            return data;

        }
        #endregion


        #region Remove
        public async Task<bool> RemoveAsync(Guid id)
        {
            if (id == Guid.Empty) throw new ArgumentException("You cannot update an empty ID.");

#if DEBUG
            logger.LogInformation("Start Db Removing........");
#endif


            bool result = await repo.RemoveAsync(id);

#if DEBUG
            logger.LogInformation("Db end Procces, Start Side Services");
#endif


            var caching = cache.IncrementAsync(CacheKeyVersion);
            var signal = hub.Clients.All.SendAsync("GPURemoved", id);

            await Task.WhenAll(caching, signal);

#if DEBUG
            logger.LogInformation("Return data");
#endif


            return result;
        }
        #endregion


        #region Mappers

        private GPUFilterModel Map(DTOGPUFilter model) => new GPUFilterModel(model.ID,
                                                                             model.Name,
                                                                             model.VideoMemorySize,
                                                                             model.VideoSlot,
                                                                             model.MemoryBus,
                                                                             model.TypeConnector,
                                                                             model.TypeMemory);

        private GPUUpdateModel Map(DTOGPUUpdateModel model) => new GPUUpdateModel(model.ID,
                                                                             model.Name,
                                                                             model.VideoMemorySize,
                                                                             model.VideoSlot,
                                                                             model.MemoryBus,
                                                                             model.TypeConnector,
                                                                             model.TypeMemory);
        private RDTOGPUSmallModel Map(GPUSmallModel model) => mapper.Map<RDTOGPUSmallModel>(model);
        private RDTOGPUModel Map(GpuDbModel model) => mapper.Map<RDTOGPUModel>(model);
        private GPUCreateModel Map(DTOGPUCreateModel model) => mapper.Map<GPUCreateModel>(
                                                                             model);

        #endregion

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
