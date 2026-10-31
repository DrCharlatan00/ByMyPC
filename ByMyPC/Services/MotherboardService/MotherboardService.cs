using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Caching;
using ByMyPC.Hubs;
using ByMyPC.Models.CpuModels.RDTO;
using ByMyPC.Models.MotherbordModels.DTO;
using ByMyPC.Models.MotherbordModels.RDTO;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace ByMyPC.Services.MotherboardService
{
    public class MotherboardService(
        IMotherboardRepo repo,
        IValidator<DTOMotherboardCreateModel> validatorCreate,
        IValidator<DTOMotherboardUpdateModel> validatorUpdate,
        IMapper mapper,
        ILogger<MotherboardService> logger,
        IHubContext<MotherboardHub> hub,
        ICacheService cacheService,
        RedisCacheDataWork redisDataWork
        ) : IMotherboardService
    {
        private readonly IMotherboardRepo repo = repo;
        private readonly IValidator<DTOMotherboardCreateModel> validatorCreate = validatorCreate;
        private readonly IValidator<DTOMotherboardUpdateModel> validatorUpdate = validatorUpdate;
        private readonly IMapper mapper = mapper;
        private readonly ILogger<MotherboardService> logger = logger;
        private readonly IHubContext<MotherboardHub> hub = hub;
        private readonly ICacheService cacheService = cacheService;
        private readonly RedisCacheDataWork redisDataWork = redisDataWork;

        #region Get
        public async Task<IEnumerable<RDTOModelMotherboardCard>> GetCardMotherboardAsync(CancellationToken cancellationToken)
        {
            ICollection<RDTOModelMotherboardCard> rdto = new List<RDTOModelMotherboardCard>();
            await foreach (var item in repo.GetCardMotherboardDbAsync(cancellationToken))
            {
                rdto.Add(Map(item));
            }
            if (rdto is null) logger.LogWarning("Data in func {func} danger is null", nameof(GetCardMotherboardAsync));
#if DEBUG
            logger.LogInformation("Retrieved data");
#endif
            return rdto;
        }

        public async Task<IEnumerable<RDTOModelMotherboard>> GetFullMotherboardAsync(CancellationToken cancellationToken)
        {
            ICollection<RDTOModelMotherboard> rdto = new List<RDTOModelMotherboard>();
            await foreach (var item in repo.GetFullMotherboardDbAsync(cancellationToken))
            {
                rdto.Add(Map(item));
            }
            if (rdto is null) logger.LogWarning("Data in func {func} danger is null", nameof(GetFullMotherboardAsync));

#if DEBUG
            logger.LogInformation("Retrieved data");
#endif
            return rdto;
        }

        public async Task<RDTOModelMotherboard?> GetByIdAsync(Guid id)
        {

#if DEBUG
            logger.LogInformation("Get by id... param : {id}",id);
#endif
            MotherboardDbModel? data = await repo.GetByIDAsync(id);
            return data != null ? Map(data) : null;
        }

        public async Task<IEnumerable<RDTOModelMotherboardCard>?> GetCardWithPaginationAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            CheckParam(page, pageSize);

#if DEBUG
            logger.LogInformation("Checking param ok, param : page {page}, pageSize {pageSize}",page,pageSize);
#endif



            const string keyCacheV = "motherboard:version";

            long version = await cacheService.GetVersionAsync(keyCacheV);


            string key = $"motherboard:v{version}:page={page}:page_size={pageSize}";

            var cacheData = await redisDataWork.GetOrSetCacheAsync(
                    key,
                    async () => {
                        IEnumerable<MotherboardSmallDbModel>? data = await repo.GetCardWithPaginationAsync(page, pageSize, cancellationToken);
                        return data.Select(Map).ToList();
                    },
                    TimeSpan.FromMinutes(3)
                );
            return cacheData ?? Enumerable.Empty<RDTOModelMotherboardCard>();

        }

        public async Task<IEnumerable<RDTOModelMotherboardCard>?> SearchByNameAsync(string name, CancellationToken cancellationToken)
        {
            CheckParam(name);

#if DEBUG
            logger.LogInformation("Check data is ok, param : {name}",name);
#endif


#if DEBUG
            logger.LogInformation("wait data in db ...., func {func db}", "SearchByNameMotherboardSmallAsync");
#endif


            var data = await repo.SearchByNameMotherboardSmallAsync(name, cancellationToken);

#if DEBUG
            logger.LogInformation("Data get in OK ");
#endif

#if DEBUG
            logger.LogInformation("Retrieveing data");
#endif



            return data != null ? data.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOModelMotherboardCard>?> SearchByNameWithPaginationAsync(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            CheckParam(page, pageSize, name);

#if DEBUG
            logger.LogInformation("Check param is ok, param : page = {page} pageSize = {pageSize} name = {name}",page,pageSize,name);
#endif

            const string keyCacheV = "motherboard:version";

            long version = await cacheService.GetVersionAsync(keyCacheV);

            string key = $"motherboard:v{version}:name={name}:page={page}:page_size={pageSize}";

            var cacheData = await redisDataWork.GetOrSetCacheAsync(
                    key,
                    async () => {
                        var data = await repo.SearchByNameMotherboardSmallWithPaginationAsync(name, page, pageSize, cancellationToken);
                        return data.Select(Map).ToList();
                    },
                    TimeSpan.FromMinutes(3)
                );
            return cacheData ?? Enumerable.Empty<RDTOModelMotherboardCard>();
        }

        public async Task<IEnumerable<RDTOModelMotherboard>?> GetByFilterAsync(DTOMotherboardFilter filter, CancellationToken cancellationToken) {

#if DEBUG
            logger.LogInformation("Func {func}, get data = {@data}",nameof(GetByFilterAsync),filter);
#endif
            
            var data = await repo.GetByFilterAsync(Map(filter),cancellationToken);

#if DEBUG
            logger.LogInformation("data get in db result is null = {result}", data is not null ? true : false);
#endif


#if DEBUG
            logger.LogInformation("Retrieving data");
#endif
            return data is not null ? data.Select(Map).ToList() : null;
        }


        public async Task<IEnumerable<RDTOModelMotherboardCard>?> GetByFilterWithPagAsync(DTOMotherboardFilter filter,int page, int pageSize, CancellationToken cancellationToken)
        {

#if DEBUG
            logger.LogInformation("Func {func} get next data = fliter {@filter} page = {page} pageSize = {pageSize}",filter,page,pageSize);
#endif
            var data = await repo.GetByFilterWithPagAsync(Map(filter), page, pageSize, cancellationToken);
#if DEBUG
            logger.LogInformation("data get in db result is null = {result}", data is not null ? true : false);
#endif


#if DEBUG
            logger.LogInformation("Retrieving data");
#endif


            return data is not null ? data.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOModelMotherboard>?> GetFullWithPag(int page, int pageSize, CancellationToken cancellationToken)
        {
            CheckParam(page, pageSize);

#if DEBUG
            logger.LogInformation("Check param is ok: param page = {page} pageSize = {pageSize}", page, pageSize);
#endif

            var data = await repo.GetFullWithPag(page, pageSize, cancellationToken);
#if DEBUG
            logger.LogInformation("data get in db result is null = {result}", data is not null ? true : false);
#endif


#if DEBUG
            logger.LogInformation("Retrieving data");
#endif
            return data is not null ? data.Select(Map).ToList() : null;
        }
        #endregion

        #region Update
        public async Task<RDTOModelMotherboard?> UpdateAsync(DTOMotherboardUpdateModel model)
        {
            await validatorUpdate.ValidateAndThrowAsync(model);

#if DEBUG
            logger.LogInformation("data is OK, Validator Skip model");
#endif

#if DEBUG
            logger.LogInformation("Start updating in DB");
#endif

            MotherboardDbModel? res = await repo.UpdateAsync(Map(model));

#if DEBUG
            logger.LogInformation(
                "Db end Updating data, return info = {@res}",
                res is not null ? res : new { Mess = "Data not updated, func returned null model" });
#endif


            if (res is null)
            {
                logger.LogInformation("Func {func} not update Motherboard with id: {id} \nModel: {@model}", nameof(UpdateAsync), model.id, model);
                return null;
            }

            var signalR = hub.Clients.All.SendAsync("MotherboardUpdated",res.ID);
            const string keyCacheV = "motherboard:version";
            var Caching =  cacheService.IncrementAsync(keyCacheV);

#if DEBUG
            logger.LogInformation("Side Services starting...");
#endif


            try
            {
                await Task.WhenAll(Caching,signalR);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Redis or SignalR error\n ex message: {mes}", ex.Message);
            }

#if DEBUG
            logger.LogInformation("Retrieving data");
#endif


            return Map(res);
        }
        #endregion

        #region Create
        public async ValueTask<Guid> CreateAsync(DTOMotherboardCreateModel model)
        {
            try
            {

                await validatorCreate.ValidateAndThrowAsync(model);

#if DEBUG
                logger.LogInformation("Data is ok, Validator skip model");
#endif


#if DEBUG
                logger.LogInformation("Procces create start in db, model information = {@model}", model);
#endif

                Guid id = await repo.CreateAsync(Map(model));

#if DEBUG
                logger.LogInformation("Motherboard created");
#endif


                var signalR = hub.Clients.All.SendAsync("NewMotherboardCreated", id);
                const string keyCacheV = "motherboard:version";
                var Caching = cacheService.IncrementAsync(keyCacheV);


#if DEBUG
                logger.LogInformation("Side Serviced started...");
#endif

                try
                {
                    await Task.WhenAll(Caching, signalR);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Redis or SignalR error\n ex message: {mes}", ex.Message);
                }

#if DEBUG
                logger.LogInformation("Retrieving data");
#endif
                return id;
            }
            catch (Exception ex) {
                logger.LogWarning(ex, "Item not created model: {@model}", model);
                throw;
            }
        }
        #endregion

        #region Remove
        public async ValueTask<bool> RemoveAsync(Guid id)
        {

#if DEBUG
            logger.LogInformation("Request delete, id = {id}",id);
#endif

#if DEBUG
            logger.LogInformation("Starting remove in db... ");
#endif

            bool result = await repo.RemoveAsync(id);

#if DEBUG
            logger.LogInformation("Removing is end in db procces, Motherboard is deleted ? = {res}",
                result == true ? "DB Send Mess is removed" : "Item not REMOVED !!!!");
#endif

            if (result == false) return false;

            var signalR = hub.Clients.All.SendAsync("MotherboardRemoved",id);
            const string keyCacheV = "motherboard:version";
            
            var Caching = cacheService.IncrementAsync(keyCacheV);

#if DEBUG
            logger.LogInformation("Side Services started...");
#endif


            try
            {
                await Task.WhenAll(Caching, signalR);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Redis or SignalR error\n ex message: {mes}", ex.Message);
            }


#if DEBUG
            logger.LogInformation("Retrieving data");
#endif


            return true;

        }
        #endregion

        #region Mapping
        private RDTOModelMotherboardCard Map(MotherboardSmallDbModel model) => mapper.Map<RDTOModelMotherboardCard>(model);
        private RDTOModelMotherboard Map(MotherboardDbModel model) => mapper.Map<RDTOModelMotherboard>(model);
        private MotherboardUpdateModel Map(DTOMotherboardUpdateModel model) => mapper.Map<MotherboardUpdateModel>(model);
        private MotherboardCreateModel Map(DTOMotherboardCreateModel model) => mapper.Map<MotherboardCreateModel>(model);
        private MotherBoardFilterModel Map(DTOMotherboardFilter model) => new MotherBoardFilterModel(model.ByName,model.ByLive,model.BySocket,model.ByHaveIntegratedGPU);
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
