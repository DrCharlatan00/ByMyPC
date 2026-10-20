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

#if DEBUG
            logger.LogInformation("-----------------Service Start Work-----------------");
#endif


            IList<RDTOPSUModel> rdto = new List<RDTOPSUModel>();
            await foreach (var item in repo.GetFullAsyncEnumerable(cancellation))
            {
                rdto.Add(Map(item));
            }

#if DEBUG
            logger.LogInformation("Data get in db and mapping is ok before check in null ");
#endif


            if (rdto is null)
            {
                logger.LogWarning("Func {func} Return null value", nameof(GetFullAsync));
            }


#if DEBUG
            logger.LogInformation("Retrieved data");
#endif

#if DEBUG
            logger.LogInformation("-----------------------Service end work------------------------");
#endif
            return rdto;
        }
        public async Task<IEnumerable<RDTOPSUSmallModel>> GetSmallAsync(CancellationToken cancellation)
        {
#if DEBUG
            logger.LogInformation("-----------------Service Start Work-----------------");
#endif
            IList<RDTOPSUSmallModel> rdto = new List<RDTOPSUSmallModel>();
            await foreach (var item in repo.GetSmallAsyncEnumerable(cancellation))
            {
                rdto.Add(Map(item));
            }
#if DEBUG
            logger.LogInformation("Data get in db and mapping is ok before check in null ");
#endif
            if (rdto is null)
            {
                logger.LogWarning("Func {func} Return null value", nameof(GetSmallAsync));
            }
#if DEBUG
            logger.LogInformation("Retrieved data");
#endif
#if DEBUG
            logger.LogInformation("-----------------------Service end work------------------------");
#endif
            return rdto;

        }

        public async Task<RDTOPSUModel?> GetByID(Guid id)
        {
#if DEBUG
            logger.LogInformation("-----------------Service Start Work-----------------");
#endif
#if DEBUG
            logger.LogInformation("GetByID Param: {id}", id);
#endif
            var item = await repo.GetByIdAsync(id);
#if DEBUG
            logger.LogInformation("-----------------------Service end work------------------------");
#endif
            return item is null ? null : Map(item);
        }

        public async Task<IEnumerable<RDTOPSUSmallModel>> SearchByName(string name, CancellationToken cancellation)
        {
#if DEBUG
            logger.LogInformation("-----------------Service Start Work-----------------");
#endif
            CheckParam(name);
#if DEBUG
            logger.LogInformation("Check data is OK Param: {name}", name);
#endif
            long versionCache = await cacheService.GetVersionAsync(keyCache);

            IEnumerable<RDTOPSUSmallModel>? dataCache = await cacheService.GetAsync<IEnumerable<RDTOPSUSmallModel>>($"psu:v{versionCache}:name={name}");

            if (dataCache is not null) return dataCache;

#if DEBUG
            logger.LogInformation("Cache is empty for PSU SearchByName");
#endif
            IList<RDTOPSUSmallModel> rdto = new List<RDTOPSUSmallModel>();
            await foreach (PSUSmallModel item in repo.SearchByNameSmallAsyncEnumerable(name, cancellation))
            {
                rdto.Add(Map(item));
            }
#if DEBUG
            logger.LogInformation("Data mapping is OK PSUSmallModel => RDTOPSUSmallModel");
#endif
            try
            {
                await cacheService.SetAsync($"psu:v{versionCache}:name={name}", rdto, TimeSpan.FromMinutes(3));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Redis not set data,First object Data {@data}", rdto.First());
            }
#if DEBUG
            logger.LogInformation("retrieved data..");
#endif
#if DEBUG
            logger.LogInformation("-----------------------Service end work------------------------");
#endif
            return rdto;
        }

        public async Task<IEnumerable<RDTOPSUSmallModel>?> GetSmallWithPag(int page, int pageSize, CancellationToken cancellation)
        {
#if DEBUG
            logger.LogInformation("-----------------Service Start Work-----------------");
#endif
            CheckParam(page, pageSize);
#if DEBUG
            logger.LogInformation("Check data is OK Param: Page {page}; PageSize {pageSize}", page, pageSize);
#endif
            long versionCache = await cacheService.GetVersionAsync(keyCache);

            IEnumerable<RDTOPSUSmallModel>? dataCache = await cacheService.GetAsync<IEnumerable<RDTOPSUSmallModel>>($"psu:v{versionCache}:page={page}:page-size:{pageSize}");

            if (dataCache is not null) return dataCache;

#if DEBUG
            logger.LogInformation("Cache is empty for PSU Small Pag");
#endif

            IEnumerable<PSUSmallModel>? data = await repo.GetSmallModelsAsyncWithPag(page, pageSize, cancellation);
            if (data is null)
                logger.LogWarning("Func {func} Return null value\nparam Page: {page}; pageSize: {pagesize}", nameof(GetSmallWithPag), page, pageSize);

#if DEBUG
            logger.LogInformation("Mapping data Small IEnumerable<PSUSmallModel>? => IEnumerable<RDTOPSUSmallModel>");
#endif
            List<RDTOPSUSmallModel>? rdto = data?.Select(Map).ToList();

#if DEBUG
            logger.LogInformation("Mapping data OK IEnumerable<PSUSmallModel>? => IEnumerable<RDTOPSUSmallModel> OK");
#endif
            try
            {
                await cacheService.SetAsync($"psu:v{versionCache}:page={page}:page-size:{pageSize}", rdto, TimeSpan.FromMinutes(3));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Redis not set data,First object Data {@data}", rdto.First());
            }

#if DEBUG
            logger.LogInformation("Data retrieved for PSU Small Pag OK");
#endif
#if DEBUG
            logger.LogInformation("-----------------------Service end work------------------------");
#endif
            return rdto;


        }

        public async Task<IEnumerable<RDTOPSUModel>> SearchByNameFullAsync(string name, CancellationToken cancellation) {
#if DEBUG
            logger.LogInformation("-----------------Service Start Work-----------------");
#endif
            CheckParam(name);
#if DEBUG
            logger.LogInformation("Check data is OK Param: {name}",name);
#endif
            IList<RDTOPSUModel> rdto = new List<RDTOPSUModel>();
            await foreach (var item in repo.SearchByNameAsyncEnumerable(name,cancellation)) 
            {
                rdto.Add(Map(item));
            }
#if DEBUG
            logger.LogInformation("retrieved data OK in Search by name Full");
#endif
#if DEBUG
            logger.LogInformation("-----------------------Service end work------------------------");
#endif
            return rdto;
        }

        public async Task<IEnumerable<RDTOPSUModel>?> GetFullWithPag(int page, int pageSize, CancellationToken cancellation)
        {
#if DEBUG
            logger.LogInformation("-----------------Service Start Work-----------------");
#endif
            CheckParam(page, pageSize);
#if DEBUG
            logger.LogInformation("Check data is OK Param : Page {page} PageSize {pageSize}",page,pageSize);
#endif
            var data = await repo.GetFullWithPag(page, pageSize, cancellation);
#if DEBUG 
    logger.LogInformation("Get data in db OK");
#endif

            if (data is null)
                logger.LogWarning("Func {func} Return null value\nparam Page: {page}; pageSize: {pagesize}", nameof(GetSmallWithPag), page, pageSize);
#if DEBUG
            logger.LogInformation("retrieved data is ok in Get full pag, Before Map");
#endif
#if DEBUG
            logger.LogInformation("-----------------------Service end work------------------------");
#endif
            return data?.Select(Map).ToList();
        }

        public async Task<IEnumerable<RDTOPSUSmallModel>> GetByFilterSmall(DTOPSUFilterModel model, CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("-----------------Service Start Work-----------------");
#endif
            PSUFilterModel filterDb = Map(model);
#if DEBUG
            logger.LogInformation("Mapping OK");
#endif
            IEnumerable<PSUSmallModel> data = await repo.GetByFilterSmall(filterDb, cancellationToken);
#if DEBUG
            logger.LogInformation("Data get in db ok, next mapping and retrieved");
#endif
#if DEBUG
            logger.LogInformation("-----------------------Service end work------------------------");
#endif
            return data.Select(Map).ToList();
        }

        public async Task<IEnumerable<RDTOPSUModel>> GetByFilterFull(DTOPSUFilterModel model, CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("-----------------Service Start Work-----------------");
#endif
            PSUFilterModel filterDb = Map(model);
#if DEBUG
            logger.LogInformation("Mapping OK");
#endif
            IEnumerable<PSUDbModel> data = await repo.GetByFilterFull(filterDb, cancellationToken);
#if DEBUG
            logger.LogInformation("Data get in db ok, next mapping and retrieved");
#endif
#if DEBUG
            logger.LogInformation("-----------------------Service end work------------------------");
#endif
            return data.Select(Map).ToList();
        }

        #endregion

        #region Update 
        public async Task<RDTOPSUModel?> UpdateAsync(DTOPSUModelUpdate model)
        {
#if DEBUG
            logger.LogInformation("-----------------Service Start Work-----------------");
#endif
#if DEBUG
            logger.LogInformation("Update func get data: {@data}", model);
#endif
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
#if DEBUG
            logger.LogInformation("Data update in db OK");
#endif
            var caching = cacheService.IncrementAsync(keyCache);
            var signal = hub.Clients.All.SendAsync("PSUCreated",data.ID);
            await Task.WhenAll(caching,signal);
#if DEBUG
            logger.LogInformation("Services get information, exception not throw");
#endif
#if DEBUG
            logger.LogInformation("-----------------------Service end work------------------------");
#endif
            return Map(data);
        }
        #endregion

        #region Create 
        public async Task<Guid> CreateAsync(DTOPSUModelCreate model)
        {
#if DEBUG
            logger.LogInformation("-----------------Service Start Work-----------------");
#endif

#if DEBUG
            logger.LogInformation("Validating data...........");
#endif


            await validator.ValidateAndThrowAsync(model);

#if DEBUG
            logger.LogInformation("Data is OK, Validator skip model");
#endif

#if DEBUG
            logger.LogInformation("Start db procces............");
#endif



            Guid id = await repo.CreateAsync(Map(model));

#if DEBUG
            logger.LogInformation("End procces in db");
#endif


            var caching = cacheService.IncrementAsync(keyCache);
            var signal = hub.Clients.All.SendAsync("PSUUpdated", id);


#if DEBUG
            logger.LogInformation("Side Service start.....");
#endif


            await Task.WhenAll(caching, signal);


#if DEBUG
            logger.LogInformation("Return id = {id}",id);
#endif
#if DEBUG
            logger.LogInformation("-----------------------Service end work------------------------");
#endif

            return id;
        }
        #endregion

        #region Remove 
        public async ValueTask<bool> RemoveAsync(Guid id)
        {

#if DEBUG
            logger.LogInformation("-----------------Service Start Work-----------------");
#endif


#if DEBUG
            logger.LogInformation("Start Db removing procces............");
#endif


            bool isDeleted = await repo.RemoveAsync(id);

#if DEBUG
            logger.LogInformation(isDeleted == false ? "Data not removed !!!!!!!!!!!!" : "Item removed");
#endif


            var caching = cacheService.IncrementAsync(keyCache);
            var signal = hub.Clients.All.SendAsync("PSUDeleted");

#if DEBUG
            logger.LogInformation("Side Services start..............");
#endif


            await Task.WhenAll(caching, signal);

#if DEBUG
            logger.LogInformation("Return data");
#endif


#if DEBUG
            logger.LogInformation("-----------------------Service end work------------------------");
#endif


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
