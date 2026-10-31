using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Caching;
using ByMyPC.Hubs;
using ByMyPC.Models.HDDModels.DTO;
using ByMyPC.Models.HDDModels.RDTO;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

namespace ByMyPC.Services.HDDService
{
    public class HDDService(IHddRepo repo,
                             IPcHddRepo pcHddRepo,
                             IMapper mapper,
                             IValidator<DTOHDDCreateModel> validator,
                             ILogger<HDDService> logger,
                             IHubContext<HDDHub> hub, 
                             ICacheService cacheService,
                             RedisCacheDataWork redisDataWork
                             ) : IHDDService
    {
        private readonly IHddRepo repo = repo;
        private readonly IPcHddRepo pcHddRepo = pcHddRepo;
        private readonly IMapper mapper = mapper;
        private readonly IValidator<DTOHDDCreateModel> validator = validator;
        private readonly ILogger<HDDService> logger = logger;
        private readonly IHubContext<HDDHub> hub = hub;
        private readonly ICacheService cacheService = cacheService;
        private readonly RedisCacheDataWork redisDataWork = redisDataWork;

        #region Get
        public async Task<IEnumerable<RDTOHDDCardModel>> GetCardModelsAsync(CancellationToken cancellationToken)
        {
            List<RDTOHDDCardModel> rdto = [];

#if DEBUG
            logger.LogInformation("Start get data in DB...");
#endif


            await foreach (var item in repo.GetSmallModelsDbAsync(cancellationToken))
            {
                rdto.Add(Map(item));
            }

#if DEBUG
            logger.LogInformation("data get in db OK and mapped, result = {res}", rdto is not null ? true : false);
#endif

            return rdto;
        }

        public async Task<IEnumerable<RDTOHDDModel>> GetModelsAsync(CancellationToken cancellationToken)
        {
            List<RDTOHDDModel> rdto = [];
#if DEBUG
            logger.LogInformation("Start get data in DB...");
#endif
            await foreach (var item in repo.GetModelsDbAsync(cancellationToken))
            {
                rdto.Add(Map(item));
            }
#if DEBUG
            logger.LogInformation("data get in db OK and mapped, result = {res}", rdto is not null ? true : false);
#endif
            return rdto;
        }

        public async Task<IEnumerable<RDTOHDDModel>?> GetFullWithPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            CheckParam(page, pageSize);

#if DEBUG
            logger.LogInformation("Check param pass data, params  page = {page} pageSize = {pageSize}",page,pageSize);
#endif


#if DEBUG
            logger.LogInformation("Start get in DB....");
#endif


            var data = await repo.GetFullWithPag(page, pageSize, cancellationToken);

#if DEBUG
            logger.LogInformation("Data get in db, next check null");
#endif


            if (data is null) {
                logger.LogWarning("With func {func} data return null",nameof(GetFullWithPagination));
                return null;
            }

#if DEBUG
            logger.LogInformation("Mapping and retrieving data");
#endif


            return [.. data.Select(Map)];

        }
        public async Task<RDTOHDDModel?> GetByIdAsync(Guid id)
        {

#if DEBUG
            logger.LogInformation("Get by id, param = {id}", id);
#endif

#if DEBUG
            logger.LogInformation("Start db proccess....");
#endif


            var item = await repo.GetByID(id);

#if DEBUG
            logger.LogInformation("DB Procces end, result = {res}",
                                  item is null ? new { Mess = "Item not get, possible not found", FuncName = "GetByID" } : "Item Found");
#endif


#if DEBUG
            logger.LogInformation("Mapping and retrieving data");
#endif


            return item is not null ? Map(item) : null;
        }

        public async Task<IEnumerable<RDTOHDDCardModel>> GetCardWithPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            CheckParam(page, pageSize);

#if DEBUG
            logger.LogInformation("Check param pass data : param page = {page}, pageSize = {pageSize}",page, pageSize);
#endif


            const string versionCacheKey = "hdd:version";
            long version = await cacheService.GetVersionAsync(versionCacheKey);

            string cacheKey = $"hdd:v{versionCacheKey}:page={page}:page-size={pageSize}";

            var cacheData = await redisDataWork.GetOrSetCacheAsync(
                    cacheKey,
                    async () => {
                        var data = await repo.GetSmallModelWithPagination(page,pageSize,cancellationToken);
                        return data.Select(Map).ToList();
                    },
                    TimeSpan.FromMinutes(3)
                );

            return cacheData ?? Enumerable.Empty<RDTOHDDCardModel>();
        }



        public async Task<IEnumerable<RDTOHDDModel>?> SearchByName(string name, CancellationToken cancellationToken)
        {
            CheckParam(name);

#if DEBUG
            logger.LogInformation("Check param pass data : Param  name = {name}",name);
#endif

#if DEBUG
            logger.LogInformation("Start db procces search by name...");
#endif

            var data = await repo.SearchByName(name, cancellationToken);

#if DEBUG
            logger.LogInformation("Db procces end, result = {res}", data is not null ? true : false);
#endif


#if DEBUG
            logger.LogInformation("Mapping and return data");
#endif


            return data is not null ? data.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOHDDCardModel>?> SearchByNameWithPag(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            CheckParam(page, pageSize, name);
#if DEBUG
            logger.LogInformation("Check param pass data : Param  name = {name} page = {page} pageSize = {pageSize}", name, page, pageSize);
#endif
            const string versionCacheKey = "hdd:version";
            long version = await cacheService.GetVersionAsync(versionCacheKey);

            string cacheKey = $"hdd:v{versionCacheKey}:name={name}:page={page}:page-size={pageSize}";
            var cacheData = await redisDataWork.GetOrSetCacheAsync(
                    cacheKey,
                    async () => {
                        var data = await repo.SearchByNameWithPag(name, page, pageSize, cancellationToken);
                        return data?.Select(Map).ToList();
                    },
                    TimeSpan.FromMinutes(3)
                );
            return cacheData ?? Enumerable.Empty<RDTOHDDCardModel>();
        }

        public async Task<IEnumerable<RDTOHDDCardModel>?> GetCardByFilterWithPag(DTOHDDFilter filter, int page, int pageSize,CancellationToken cancellationToken)
        {
            CheckParam(page, pageSize);
#if DEBUG
            logger.LogInformation("Check param pass data : Param  page = {page} pageSize = {pageSize}", page, pageSize);
#endif

#if DEBUG
            logger.LogInformation("Mapping filter....");
#endif

            HDDFilterModel filterDB = Map(filter);


#if DEBUG
            logger.LogInformation("filter to db format OK");
#endif


#if DEBUG
            logger.LogInformation("Start db procces...");
#endif


            var result = await repo.GetSmallByFilter(filterDB,page,pageSize, cancellationToken);

#if DEBUG
            logger.LogInformation("end db procces, result = {res}",result is not null ? true : false);
#endif


#if DEBUG
            logger.LogInformation("Mapping and return data");
#endif


            return result is not null ? result.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOHDDModel>?> GetByFilter(DTOHDDFilter filter, CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("Mapping filter....");
#endif

            HDDFilterModel filterDB = Map(filter);
#if DEBUG
            logger.LogInformation("filter to db format OK");
#endif
#if DEBUG
            logger.LogInformation("Start db procces...");
#endif

            var result = await repo.GetByFilter(filterDB, cancellationToken);
#if DEBUG
            logger.LogInformation("end db procces, result = {res}", result is not null ? true : false);
#endif
#if DEBUG
            logger.LogInformation("Mapping and return data");
#endif

            return result is not null ? result.Select(Map).ToList() : null;
        }

        #endregion

        #region Update

        public async Task<RDTOHDDModel?> UpdateAsync(DTOHDDUpdateModel model)
        {

#if DEBUG
            logger.LogInformation("Start proccesing Update, Get model {model}",model);
#endif

#if DEBUG
            logger.LogInformation("Mapping model...");
#endif


            HDDUpdateModel updateModel = new HDDUpdateModel(model.id, model.name, model.GbSize, (HddConnector?)model.ConnectorType);

#if DEBUG
            logger.LogInformation("data is mapped");
#endif


#if DEBUG
            logger.LogInformation("Start db procces update....");
#endif

            var result = await repo.UpdateAsync(updateModel);

#if DEBUG
            logger.LogInformation("Procces update is end");
#endif

            if (result is not  null)
            {
                var signalR = hub.Clients.All.SendAsync("HDDUpdated", result.ID);
                var RedisUpdate = cacheService.IncrementAsync("hdd:version");
#if DEBUG
                logger.LogInformation("Side Services start.....");
#endif
                await Task.WhenAll(signalR,RedisUpdate);

#if DEBUG
                logger.LogInformation("Mapping end return data");
#endif
                return Map(result) ;

            }
            logger.LogWarning("In func {func} update db return null with param model: {@model}\nConvertedModel: {@conv}",
                  nameof(UpdateAsync),
                  model,
                  updateModel);
            return null;
        }

        #endregion


        #region Create
        public async ValueTask<Guid> CreateAsync(DTOHDDCreateModel dto)
        {

#if DEBUG
            logger.LogInformation("Request Create new HDD: param {dto}",dto);
#endif


#if DEBUG
            logger.LogInformation("Start validate data....");
#endif


            await validator.ValidateAndThrowAsync(dto);

#if DEBUG
            logger.LogInformation("DTO Is Ok, Validator skip DTO");
#endif


#if DEBUG
            logger.LogInformation("Mapping DTO...");
#endif


            var model = Map(dto);

#if DEBUG
            logger.LogInformation("DTO is mapped");
#endif

#if DEBUG
            logger.LogInformation("Start creating in db...");
#endif

            Guid id = await repo.CreateAsync(model);


#if DEBUG
            logger.LogInformation("Procces create is end");
#endif

#if DEBUG
            logger.LogInformation("Side Services start");
#endif



            var RedisUpdate = cacheService.IncrementAsync("hdd:version");
            var signalR = hub.Clients.All.SendAsync("HDDCreated", id);
            await Task.WhenAll(signalR, RedisUpdate);

#if DEBUG
            logger.LogInformation("Return ID = {id}",id);
#endif


            return id;
        }

        public async Task<Guid> CreateAndAttachAsync(DTOHDDCreateModel dto, Guid pcId) {
#if DEBUG
            logger.LogInformation("Request Create new HDD: param {dto}", dto);
#endif


#if DEBUG
            logger.LogInformation("Start validate data....");
#endif
            await validator.ValidateAndThrowAsync(dto);

#if DEBUG
            logger.LogInformation("DTO is Ok , validator skip dto");
#endif


#if DEBUG
            logger.LogInformation("DTO Mapping to Db format");
#endif


            var model = Map(dto);

#if DEBUG
            logger.LogInformation("DTO Is mapped");
#endif

#if DEBUG
            logger.LogInformation("Start Creating and Attach in db....");
#endif



            Guid id = await repo.CreateAndAttach(model,pcId);

#if DEBUG
            logger.LogInformation("Procces in db end");
#endif


            var RedisUpdate = cacheService.IncrementAsync("hdd:version");
            var signalR = hub.Clients.All.SendAsync("HDDCreated", id);

#if DEBUG
            logger.LogInformation("Side Services started...");
#endif


            await Task.WhenAll(signalR, RedisUpdate);

#if DEBUG
            logger.LogInformation("Return id = {id}", id);
#endif


            return id;
        }
        #endregion

        #region Remove
        public async ValueTask<bool> RemoveAsync(Guid id)
        {

#if DEBUG
            logger.LogInformation("Request remove item with id = {id}", id);
#endif


#if DEBUG
            logger.LogInformation("Start db removing...");
#endif


            var ans = await repo.RemoveAsync(id);

#if DEBUG
            logger.LogInformation("Db end removing");
#endif


            if (ans == false) return false;

#if DEBUG
            logger.LogInformation("Item possible removed");
#endif



            var signalR = hub.Clients.All.SendAsync("HDDRemoved", id);
            var RedisUpdate = cacheService.IncrementAsync("hdd:version");

#if DEBUG
            logger.LogInformation("Side services start...");
#endif


            await Task.WhenAll(signalR, RedisUpdate);

#if DEBUG
            logger.LogInformation("return result");
#endif


            return true;
        }
        #endregion

        #region Other Opertaions
        public async ValueTask<bool> AttachHdd(Guid pcId, Guid HddId)
        {

#if DEBUG
            logger.LogInformation("Start Attach hdd {id} => PC {id}",HddId, pcId);
#endif

            return await pcHddRepo.AttachHddToPc(pcId, HddId);

        }
        public async ValueTask<bool> DeattachHdd(Guid pcId, Guid HddId)
        {

#if DEBUG
            logger.LogInformation("Start DeAttach hdd {id} !=> PC {id}", HddId, pcId);
#endif


            return await pcHddRepo.DeAtthachHDD(pcId, HddId);
        }

        #endregion

        #region Mapping
        private RDTOHDDCardModel Map(HDDSmallModel model) => mapper.Map<RDTOHDDCardModel>(model);
        private RDTOHDDModel Map(HDDDbModel model) => mapper.Map<RDTOHDDModel>(model);

        private HDDFilterModel Map(DTOHDDFilter model) => new HDDFilterModel(
            model.Name,
            model.GbSize,
            (HddConnector?)model.Connector
            );
        private HDDCreateModel Map(DTOHDDCreateModel model) => mapper.Map<HDDCreateModel>(model);
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
