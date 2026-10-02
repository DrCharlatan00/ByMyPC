using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Caching;
using ByMyPC.Hubs;
using ByMyPC.Models.PSUModels;
using ByMyPC.Models.PSUModels.DTO;
using ByMyPC.Models.PSUModels.RDTO;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ByMyPC.Services.PSUService
{
    public class PSUService(
            IPSURepo repo,
            ILogger<PSUService> logger,
            IValidator<DTOPSUModelCreate> validator,
            IMapper mapper,
            ICacheService cacheService,
            IHubContext<PSUHub> hub
        ) : IPSUService
    {

        private readonly IPSURepo repo = repo;
        private readonly ILogger<PSUService> logger = logger;
        private readonly IValidator<DTOPSUModelCreate> validator = validator;
        private readonly IMapper mapper = mapper;
        private readonly ICacheService cacheService = cacheService;
        private readonly IHubContext<PSUHub> hub = hub;
        const string keyCache = "psu:version";

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
            long versionCache = await cacheService.GetVersionAsync(keyCache);

            IEnumerable<RDTOPSUSmallModel>? dataCache = await cacheService.GetAsync<IEnumerable<RDTOPSUSmallModel>>($"psu:v{versionCache}:name={name}");

            if (dataCache is not null) return dataCache;

            IList<RDTOPSUSmallModel> rdto = new List<RDTOPSUSmallModel>();
            await foreach (var item in repo.SearchByNameSmallAsyncEnumerable(name, cancellation))
            {
                rdto.Add(Map(item));
            }

            try
            {
                await cacheService.SetAsync($"psu:v{versionCache}:name={name}", rdto, TimeSpan.FromMinutes(3));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Redis not set data,First object Data {@data}", rdto.First());
            }
            

            return rdto;
        }

        public async Task<IEnumerable<RDTOPSUSmallModel>?> GetSmallWithPag(int page, int pageSize, CancellationToken cancellation)
        {

            long versionCache = await cacheService.GetVersionAsync(keyCache);

            IEnumerable<RDTOPSUSmallModel>? dataCache = await cacheService.GetAsync<IEnumerable<RDTOPSUSmallModel>>($"psu:v{versionCache}:page={page}:page-size:{pageSize}");

            if (dataCache is not null) return dataCache;

            var data = await repo.GetSmallModelsAsyncWithPag(page, pageSize, cancellation);
            if (data is null)
                logger.LogWarning("Func {func} Return null value\nparam Page: {page}; pageSize: {pagesize}", nameof(GetSmallWithPag), page, pageSize);
            List<RDTOPSUSmallModel>? rdto = data?.Select(Map).ToList();

            try
            {
                await cacheService.SetAsync($"psu:v{versionCache}:page={page}:page-size:{pageSize}", rdto, TimeSpan.FromMinutes(3));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Redis not set data,First object Data {@data}", rdto.First());
            }

            return rdto;


        }

        public async Task<IEnumerable<RDTOPSUModel>> SearchByNameFullAsync(string name, CancellationToken cancellation) {
            IList<RDTOPSUModel> rdto = new List<RDTOPSUModel>();
            await foreach (var item in repo.SearchByNameAsyncEnumerable(name,cancellation)) 
            {
                rdto.Add(Map(item));
            }
            return rdto;
        }

        public async Task<IEnumerable<RDTOPSUModel>?> GetFullWithPag(int page, int pageSize, CancellationToken cancellation)
        {
            var data = await repo.GetFullWithPag(page, pageSize, cancellation);
            if (data is null)
                logger.LogWarning("Func {func} Return null value\nparam Page: {page}; pageSize: {pagesize}", nameof(GetSmallWithPag), page, pageSize);
            return data?.Select(Map).ToList();
        }

        public async Task<IEnumerable<RDTOPSUSmallModel>> GetByFilterSmall(DTOPSUFilterModel model, CancellationToken cancellationToken)
        {
            PSUFilterModel filterDb = Map(model);
            IEnumerable<PSUSmallModel> data = await repo.GetByFilterSmall(filterDb, cancellationToken);
            return data.Select(Map).ToList();
        }

        public async Task<IEnumerable<RDTOPSUModel>> GetByFilterFull(DTOPSUFilterModel model, CancellationToken cancellationToken)
        {
            PSUFilterModel filterDb = Map(model);
            IEnumerable<PSUDbModel> data = await repo.GetByFilterFull(filterDb, cancellationToken);
            return data.Select(Map).ToList();
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
            if (data is null) return null;
            var caching = cacheService.IncrementAsync(keyCache);
            var signal = hub.Clients.All.SendAsync("PSUCreated",data.ID);
            await Task.WhenAll(caching,signal);
            return Map(data);
        }
        #endregion

        #region Create 
        public async Task<Guid> CreateAsync(DTOPSUModelCreate model)
        {
            await validator.ValidateAndThrowAsync(model);
            Guid id = await repo.CreateAsync(Map(model));
            var caching = cacheService.IncrementAsync(keyCache);
            var signal = hub.Clients.All.SendAsync("PSUUpdated", id);
            await Task.WhenAll(caching, signal);
            return id;
        }
        #endregion

        #region Remove 
        public async ValueTask<bool> RemoveAsync(Guid id)
        {
            bool isDeleted = await repo.RemoveAsync(id);
            var caching = cacheService.IncrementAsync(keyCache);
            var signal = hub.Clients.All.SendAsync("PSUDeleted");
            await Task.WhenAll(caching, signal);
            return isDeleted;
        }
        #endregion 

        #region Mappers
        private RDTOPSUModel Map(PSUDbModel model) => mapper.Map<RDTOPSUModel>(model);
        private RDTOPSUSmallModel Map(PSUSmallModel model) => mapper.Map<RDTOPSUSmallModel>(model);
        private PSUCreateModel Map(DTOPSUModelCreate model) => mapper.Map<PSUCreateModel>(model);
        private PSUFilterModel Map(DTOPSUFilterModel model) => new PSUFilterModel(model.Name,
                                                                                  model.PowerWatt,
                                                                                  model.IsLive,
                                                                                  model.Size,
                                                                                  model.IsModular,
                                                                                  model.IsCertified);
        #endregion
    }
}
