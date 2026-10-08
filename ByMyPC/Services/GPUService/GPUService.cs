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
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Abstractions;
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

        public async Task<RDTOGPUModel?> GetByIDAsync(Guid id)
        {
            var item = await repo.GetByID(id);
            return item == null ? null : Map(item);
        }

        public async Task<IEnumerable<RDTOGPUSmallModel>?> GetSmallWithPag(int page, int pageSize, CancellationToken cancellationToken)
        {
            if (page >= 10000) throw new ArgumentException("Page is to big");
            if (pageSize >= 10000) throw new ArgumentException("Page Size is not correct");

            var keyVer = await cache.GetVersionAsync(CacheKeyVersion);

            string CacheKey = $"gpu:v{keyVer}:page={page}:pageSize={pageSize}";

            var cacheData = await cache.GetAsync<IEnumerable<RDTOGPUSmallModel>>(CacheKey);

            if (cacheData is not null) return cacheData;

            var data = await repo.GetSmallModelWithPagination(page, pageSize, cancellationToken);

            if (data is null)
            {
                logger.LogInformation("For request with param page {page} and pageSize {size} , returns data is null", page, pageSize);
                return null;
            }

            var dataR = data.Select(Map).ToList();
            await cache.SetAsync(CacheKey,dataR,TimeSpan.FromMinutes(3));

            return dataR;
        }


        public async Task<IEnumerable<RDTOGPUModel>?> SearchByNameAsync(string name, CancellationToken cancellationToken)
        {
            var data = await repo.SearchByName(name, cancellationToken);
            if (data is null)
            {
                logger.LogInformation("For request {func} with param name: {name} , returns data is null", nameof(SearchByNameAsync), name);
                return null;
            }
            return data.Select(Map).ToList();
        }

        public async Task<IEnumerable<RDTOGPUSmallModel>?> SearchByNameAsyncWithPag(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            var keyVer = await cache.GetVersionAsync(CacheKeyVersion);

            string CacheKey = $"gpu:v{keyVer}:name={name}:page={page}:pageSize={pageSize}";

            var cacheData = await cache.GetAsync<IEnumerable<RDTOGPUSmallModel>>(CacheKey);

            if (cacheData is not null) return cacheData;

            var data = await repo.SearchByNameWithPag(name, page, pageSize, cancellationToken);
            if (data is null)
            {
                logger.LogInformation("For request {func} with param name: {name} , returns data is null", nameof(SearchByNameAsyncWithPag), name);
                return null;
            }


            var dataR = data.Select(Map).ToList();
            await cache.SetAsync(CacheKey, dataR, TimeSpan.FromMinutes(3));
            return dataR;
        }

        public async Task<IEnumerable<RDTOGPUModel>> GetByFilterAsync(DTOGPUFilter filter, CancellationToken cancellationToken)
        {
            var data = await repo.GetByFilter(Map(filter), cancellationToken);
            return data.Select(Map).ToList();
        }

        public async Task<IEnumerable<RDTOGPUSmallModel>> GetByFilterWithPagAsync(DTOGPUFilter filter, int page, int pageSize, CancellationToken cancellationToken)
        {
            var keyVer = await cache.GetVersionAsync(CacheKeyVersion);

            string CacheKey = $"gpu:v{keyVer}:filter{filter}:page={page}:pageSize={pageSize}";

            var cacheData = await cache.GetAsync<IEnumerable<RDTOGPUSmallModel>>(CacheKey);

            if (cacheData is not null) return cacheData;
            IEnumerable<GPUSmallModel> data = await repo.GetSmallByFilter(Map(filter), page, pageSize, cancellationToken);
            
            var dataR = data.Select(Map).ToList();
            await cache.SetAsync(CacheKey, dataR, TimeSpan.FromMinutes(3));
            
            return dataR;
        }
        #endregion

        #region Update
        public async Task<RDTOGPUModel?> UpdateAsync(DTOGPUUpdateModel model)
        {
            if (model.ID == Guid.Empty) throw new ArgumentException("You cannot update an empty ID.");

            var data = await repo.UpdateAsync(Map(model));

            if (data is null)
            {
                return null;
            }
            else
            {
                var caching = cache.IncrementAsync(CacheKeyVersion);
                var signal = hub.Clients.All.SendAsync("GPUUpdated", data.ID);

                await Task.WhenAll(caching, signal);
                return Map(data);
            }
        }
        #endregion

        #region Create
        public async ValueTask<Guid> CreateAsync(DTOGPUCreateModel model)
        {
            await validator.ValidateAndThrowAsync(model);

            var data = await repo.CreateAsync(Map(model));
            
            var caching = cache.IncrementAsync(CacheKeyVersion);
            var signal = hub.Clients.All.SendAsync("GPUCreated",data);

            await Task.WhenAll(caching,signal);
            return data;

        }
        #endregion


        #region Remove
        public async Task<bool> RemoveAsync(Guid id)
        {
            if (id == Guid.Empty) throw new ArgumentException("You cannot update an empty ID.");

            bool result = await repo.RemoveAsync(id);
            var caching = cache.IncrementAsync(CacheKeyVersion);
            var signal = hub.Clients.All.SendAsync("GPURemoved", id);

            await Task.WhenAll(caching, signal);
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
    }
}
