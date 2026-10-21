using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Caching;
using ByMyPC.Hubs;
using ByMyPC.Models.RAMModels.DTO;
using ByMyPC.Models.RAMModels.RDTO;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.SignalR;
using System.Data;
using System.Runtime.InteropServices;

namespace ByMyPC.Services.RamService
{
    public class RamService(
            IRamRepo repo,
            IPcRamRepo ramRepo,
            ILogger<RamService> logger,
            IValidator<DTORAMCreate> validatorCreate,
            IMapper mapper,
            IHubContext<RamHub> hub,
            ICacheService cacheService
        ) : IRamService
    {
        private const string keyVersionCache = "ram:version";
        private readonly IRamRepo repo = repo;
        private readonly IPcRamRepo ramRepo = ramRepo;
        private readonly ILogger<RamService> logger = logger;
        private readonly IValidator<DTORAMCreate> validatorCreate = validatorCreate;
        private readonly IMapper mapper = mapper;
        private readonly IHubContext<RamHub> hub = hub;
        private readonly ICacheService cacheService = cacheService;

        #region Get
        public async Task<IEnumerable<RDTORAMModel>> GetFullAsync(CancellationToken cancellationToken)
        {
            IList<RDTORAMModel> rdto = new List<RDTORAMModel>();

#if DEBUG
            logger.LogInformation("Start Get in DB Async Enumerable and Mapping........");
#endif


            await foreach (var item in repo.GetFullAsyncEnumerable(cancellationToken))
            {
                rdto.Add(Map(item));
            }

#if DEBUG
            logger.LogInformation("Db end Work");
#endif
            return rdto;


        }

        public async Task<IEnumerable<RDTORAMSmallModel>> GetSmallAsync(CancellationToken cancellationToken)
        {
            IList<RDTORAMSmallModel> rdto = new List<RDTORAMSmallModel>();

#if DEBUG
            logger.LogInformation("Start Get in DB Async Enumerable and Mapping........");
#endif


            await foreach (var item in repo.GetSmallAsyncEnumerable(cancellationToken))
            {
                rdto.Add(Map(item));
            }

#if DEBUG
            logger.LogInformation("Db end Work");
#endif
            return rdto;

        }

        public async Task<RDTORAMModel?> GetByIdAsync(Guid id)
        {

#if DEBUG
            logger.LogInformation("Start Get by ID {id} in db.......", id);
#endif


            var item = await repo.GetByIdAsync(id);

#if DEBUG
            logger.LogInformation("End Work db... Mapping? = {MapOrNo}", item is not null ? true : false);
#endif


            return item is not null ? Map(item) : null;
        }

        public async Task<IEnumerable<RDTORAMModel>?> GetFullWithPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            CheckParam(page, pageSize);

#if DEBUG
            logger.LogInformation("CheckParam pass, param  page = {page} pageSize = {pageSize}", page, pageSize);
#endif

#if DEBUG
            logger.LogInformation("try get in cache.......");
#endif


            long vers = await cacheService.GetVersionAsync(keyVersionCache);

            string key = $"ram:v{vers}:page={page}:pageSize={pageSize}";

            var cache = await cacheService.GetAsync<IEnumerable<RDTORAMModel>>(key);

            if (cache is not null) return cache;

#if DEBUG
            logger.LogInformation("Cache is miss");
#endif


#if DEBUG
            logger.LogInformation("Start pagination in db............");
#endif


            var data = await repo.GetFullWithPagAsync(page, pageSize, cancellationToken);


#if DEBUG
            logger.LogInformation("End Db Work");
#endif
            if (data is null)
            {
                logger.LogWarning("data with full return null , param  page = {page} pageSize = {pageSize}", page, pageSize);
                return null;
            }


#if DEBUG
            logger.LogInformation("Mapping data.......");
#endif
            var mappedData = data.Select(Map).ToList();


#if DEBUG
            logger.LogInformation("Caching......");
#endif
            await cacheService.SetAsync(key,mappedData,TimeSpan.FromMinutes(3));



            return mappedData;

        }


        public async Task<IEnumerable<RDTORAMSmallModel>?> GetSmallWithPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            CheckParam(page, pageSize);

#if DEBUG
            logger.LogInformation("CheckParam pass, param  page = {page} pageSize = {pageSize}", page, pageSize);
#endif
            long vers = await cacheService.GetVersionAsync(keyVersionCache);

            string key = $"ram:v{vers}:page={page}:pageSize={pageSize}";

            var cache = await cacheService.GetAsync<IEnumerable<RDTORAMSmallModel>>(key);

            if (cache is not null) return cache;

#if DEBUG
            logger.LogInformation("Cache is miss");
#endif
#if DEBUG
            logger.LogInformation("Start pagination in db............");
#endif


            var data = await repo.GetSmallWithPagAsync(page, pageSize, cancellationToken);


#if DEBUG
            logger.LogInformation("End Db Work");
#endif
            if (data is null)
            {
                logger.LogWarning("data with full return null , param  page = {page} pageSize = {pageSize}", page, pageSize);
                return null;
            }


#if DEBUG
            logger.LogInformation("Mapping data.......");
#endif
            var mappedData = data.Select(Map).ToList();


#if DEBUG
            logger.LogInformation("Caching......");
#endif
            await cacheService.SetAsync(key, mappedData, TimeSpan.FromMinutes(3));



            return mappedData;


        }

        public async Task<IEnumerable<RDTORAMModel>?> SearchByNameAsyncFull(string name, CancellationToken cancellation)
        {
            CheckParam(name);
#if DEBUG
            logger.LogInformation("CheckParam pass, Param name = {name}", name);
#endif


#if DEBUG
            logger.LogInformation("Start Search in DB.............");
#endif


            var data = await repo.SearchByNameAsync(name, cancellation);

#if DEBUG
            logger.LogInformation("End Db Procees");
#endif

            if (data is null)
            {
                logger.LogWarning("Search name: {name} return null", name);
                return null;
            }

            return data.Select(Map).ToList();

        }



        public async Task<IEnumerable<RDTORAMSmallModel>?> SearchByNameAsyncSmallPaginated(string name, int page, int pageSize, CancellationToken cancellation)
        {
            CheckParam(name);
#if DEBUG
            logger.LogInformation("CheckParam pass, Param name = {name}", name);
#endif

#if DEBUG
            logger.LogInformation("Try get in cache..........");
#endif
            long vers = await cacheService.GetVersionAsync(keyVersionCache);
            string key = $"rams:{vers}:name={name}:page={page}:pageSize={pageSize}";
            var cache = await cacheService.GetAsync<IEnumerable<RDTORAMSmallModel>>(key);
            if (cache is not null) return cache;


#if DEBUG
            logger.LogInformation("cache is miss");
#endif


#if DEBUG
            logger.LogInformation("Start Search in DB.............");
#endif

            var data = await repo.SearchByNameWithPagAsync(name, page, pageSize, cancellation);

#if DEBUG
            logger.LogInformation("End Db Procees");
#endif

            if (data is null)
            {
                logger.LogWarning("Search name: {name} return null", name);
                return null;
            }

#if DEBUG
            logger.LogInformation("Mapping.......");
#endif


            List<RDTORAMSmallModel> mappedData = data.Select(Map).ToList();


#if DEBUG
            logger.LogInformation("Caching......");
#endif
            await cacheService.SetAsync(key,mappedData,TimeSpan.FromMinutes(3));

            return mappedData;

        }


        public async Task<IEnumerable<RDTORAMModel>> GetFullByFilterAsync(DTORAMFilter filter, CancellationToken cancellationToken)
        {

#if DEBUG
            logger.LogInformation("Start Mapping and Filtering in Db...........");
#endif

            var data = await repo.GetByFilterAsync(Map(filter), cancellationToken);

#if DEBUG
            logger.LogInformation("Db end Work");
#endif

            return data is not null ? data.Select(Map).ToList() : Enumerable.Empty<RDTORAMModel>();

        }

        public async Task<IEnumerable<RDTORAMSmallModel>> GetSmallByFilterAsyncPaginated(DTORAMFilter filter, int page, int pageSize, CancellationToken cancellationToken)
        {

#if DEBUG
            logger.LogInformation("Start Mapping and Filtering in Db...........");
#endif

            var data = await repo.GetByFilterWithPagAsync(Map(filter), page, pageSize, cancellationToken);

#if DEBUG
            logger.LogInformation("Db end Work");
#endif

            return data is not null ? data.Select(Map).ToList() : Enumerable.Empty<RDTORAMSmallModel>();

        }
        #endregion

        #region Update
        public async Task<RDTORAMModel?> UpdateAsync(DTORAMUpdate update)
        {

#if DEBUG
            logger.LogInformation("Start Procces Update, Param model = {modelUpdate}", update);
#endif


#if DEBUG
            logger.LogInformation("Db Start Updating.....");
#endif


            var ans = await repo.UpdateAsync(Map(update));


#if DEBUG
            logger.LogInformation("Db Update end");
#endif

            if (ans is null) return null;

            var hubWork = hub.Clients.All.SendAsync("RamUpdated",ans.ID);
            var cacheWork = cacheService.IncrementAsync(keyVersionCache);
            await Task.WhenAll(hubWork, cacheWork);
            return Map(ans);

        }
        #endregion

        #region Create
        public async ValueTask<Guid> CreateAsync(DTORAMCreate dto)
        {
            await validatorCreate.ValidateAndThrowAsync(dto);

#if DEBUG
            logger.LogInformation("Validator pass DTO = {@dto}", dto);
#endif


#if DEBUG
            logger.LogInformation("Start Procces creating in Db............. ");
#endif

            Guid id = await repo.CreateAsync(Map(dto));

#if DEBUG
            logger.LogInformation("Procces create in db end");
#endif
            var hubWork = hub.Clients.All.SendAsync("RamCreated",id);
            var cacheWork = cacheService.IncrementAsync(keyVersionCache);
            await Task.WhenAll(hubWork, cacheWork);
            return id;


        }

        #endregion

        #region Remove
        public async ValueTask<bool> RemoveAsync(Guid id)
        {

#if DEBUG
            logger.LogInformation("Start removing in db, ID = {id}", id);
#endif


            bool ans = await repo.RemoveAsync(id);

#if DEBUG
            logger.LogInformation("Item is Removed? = {result}", ans);
#endif
            var hubWork = hub.Clients.All.SendAsync("RamRemoved", id);
            var cacheWork = cacheService.IncrementAsync(keyVersionCache);
            await Task.WhenAll(hubWork, cacheWork);
            return ans;
        }
        #endregion

        #region Others
        public async ValueTask<bool> AttachAsync(Guid pcID, Guid RamID)
        {

#if DEBUG
            logger.LogInformation("Attaching pc {pcid} and ram {ramID}", pcID, RamID);
#endif
            bool result = await ramRepo.AttachAsync(pcID, RamID);


#if DEBUG
            logger.LogInformation("Attaching end, result = {res}", result);
#endif
            if (result) await hub.Clients.All.SendAsync("RamAttached",RamID);
            return result;


        }

        public async ValueTask<bool> DeAttachAsync(Guid pcID, Guid RamID)
        {

#if DEBUG
            logger.LogInformation("DeAttaching pc {pcid} and ram {ramID}", pcID, RamID);
#endif
            bool result = await ramRepo.DeAttachAsync(pcID, RamID);


#if DEBUG
            logger.LogInformation("DeAttaching end, result = {res}", result);
#endif
            if (result) await hub.Clients.All.SendAsync("RamDeattached", RamID);
            return result;


        }
        #endregion

        #region Mappers
        private RDTORAMModel Map(RamDbModel model) => mapper.Map<RDTORAMModel>(model);

        private RDTORAMSmallModel Map(RamSmallModel model) => mapper.Map<RDTORAMSmallModel>(model);

        private RamFilterModel Map(DTORAMFilter filter) => new RamFilterModel(
            filter.name,
            filter.size,
            filter.Type,
            filter.frequency,
            filter.isLive
            );
        private RamUpdateModel Map(DTORAMUpdate model) => new RamUpdateModel(
                model.id,
                model.name,
                model.size,
                model.Type,
                model.frequency,
                model.isLive
            );
        private RamCreateModel Map(DTORAMCreate model) => mapper.Map<RamCreateModel>(model);
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
