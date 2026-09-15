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
using Microsoft.AspNetCore.SignalR;

namespace ByMyPC.Services.HDDService
{
    public class HDDService(IHddRepo repo,
                             IPcHddRepo pcHddRepo,
                             IMapper mapper,
                             IValidator<DTOHDDCreateModel> validator,
                             ILogger<HDDService> logger,
                             IHubContext<HDDHub> hub, 
                             ICacheService cacheService) : IHDDService
    {
        private readonly IHddRepo repo = repo;
        private readonly IPcHddRepo pcHddRepo = pcHddRepo;
        private readonly IMapper mapper = mapper;
        private readonly IValidator<DTOHDDCreateModel> validator = validator;
        private readonly ILogger<HDDService> logger = logger;
        private readonly IHubContext<HDDHub> hub = hub;
        private readonly ICacheService cacheService = cacheService;

        #region Get
        public async Task<IEnumerable<RDTOHDDCardModel>> GetCardModelsAsync(CancellationToken cancellationToken)
        {
            List<RDTOHDDCardModel> rdto = [];
            await foreach (var item in repo.GetSmallModelsDbAsync(cancellationToken))
            {
                rdto.Add(Map(item));
            }
            return rdto;
        }

        public async Task<IEnumerable<RDTOHDDModel>> GetModelsAsync(CancellationToken cancellationToken)
        {
            List<RDTOHDDModel> rdto = [];
            await foreach (var item in repo.GetModelsDbAsync(cancellationToken))
            {
                rdto.Add(Map(item));
            }
            return rdto;
        }

        public async Task<RDTOHDDModel?> GetByIdAsync(Guid id)
        {
            var item = await repo.GetByID(id);
            return item is not null ? Map(item) : null;
        }

        public async Task<IEnumerable<RDTOHDDCardModel>> GetCardWithPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            const string versionCacheKey = "hdd:version";
            long version = await cacheService.GetVersionAsync(versionCacheKey);

            string cacheKey = $"hdd:v{versionCacheKey}:page={page}:page-size={pageSize}";


            var cache = await cacheService.GetAsync<IEnumerable<RDTOHDDCardModel>>(cacheKey);

            if (cache is not null) return cache;

            var data = await repo.GetSmallModelWithPagination(page, pageSize, cancellationToken);
            if (data is null)
            {
                logger.LogError("In func {func} get data is null with param page: {page} pageSize: {pagesize}", nameof(GetCardWithPagination), page, pageSize);
                throw new NullReferenceException("In pag method get null");
            }
            var RDTOData = data.Select(Map).ToList();
            await cacheService.SetAsync(cacheKey, RDTOData, TimeSpan.FromMinutes(2));
            return RDTOData;
        }

        public async Task<IEnumerable<RDTOHDDModel>?> SearchByName(string name, CancellationToken cancellationToken)
        {
            var data = await repo.SearchByName(name, cancellationToken);
            return data is not null ? data.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOHDDCardModel>?> SearchByNameWithPag(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            const string versionCacheKey = "hdd:version";
            long version = await cacheService.GetVersionAsync(versionCacheKey);

            string cacheKey = $"hdd:v{versionCacheKey}:name={name}:page={page}:page-size={pageSize}";
            var cache = await cacheService.GetAsync<IEnumerable<RDTOHDDCardModel>>(cacheKey);
            
            if (cache is not null) return cache;

            var data = await repo.SearchByNameWithPag(name, page, pageSize, cancellationToken);
            if (data is not null) {
                await cacheService.SetAsync(cacheKey, data, TimeSpan.FromMinutes(2));
                return data.Select(Map).ToList();
            }
            return null;
        }

        public async Task<IEnumerable<RDTOHDDCardModel>?> GetCardByFilterWithPag(DTOHDDFilter filter, int page, int pageSize,CancellationToken cancellationToken)
        {
            HDDFilterModel filterDB = filter.ConvertToDbModel(filter);
            var result = await repo.GetSmallByFilter(filterDB,page,pageSize, cancellationToken);
            return result is not null ? result.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOHDDModel>?> GetByFilter(DTOHDDFilter filter, CancellationToken cancellationToken)
        {
            HDDFilterModel filterDB = filter.ConvertToDbModel(filter);
            var result = await repo.GetByFilter(filterDB, cancellationToken);
            return result is not null ? result.Select(Map).ToList() : null;
        }

        #endregion

        #region Update

        public async Task<RDTOHDDModel?> UpdateAsync(DTOHDDUpdateModel model)
        {
            HDDUpdateModel updateModel = new HDDUpdateModel(model.id, model.name, model.GbSize, (HddConnector?)model.ConnectorType);
            var result = await repo.UpdateAsync(updateModel);
            if (result is not  null)
            {
                var signalR = hub.Clients.All.SendAsync("HDDUpdated", result.ID);
                var RedisUpdate = cacheService.IncrementAsync("hdd:version");
                await Task.WhenAll(signalR,RedisUpdate);
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
        public async Task<Guid> CreateAsync(DTOHDDCreateModel dto)
        {
            await validator.ValidateAndThrowAsync(dto);
            var model = Map(dto);
            Guid id = await repo.CreateAsync(model);
            var RedisUpdate = cacheService.IncrementAsync("hdd:version");
            var signalR = hub.Clients.All.SendAsync("HDDCreated", id);
            await Task.WhenAll(signalR, RedisUpdate);
            return id;
        }

        public async Task<Guid> CreateAndAttachAsync(DTOHDDCreateModel dto, Guid pcId) {
            await validator.ValidateAndThrowAsync(dto);
            var model = Map(dto);
            Guid id = await repo.CreateAndAttach(model,pcId);
            var RedisUpdate = cacheService.IncrementAsync("hdd:version");
            var signalR = hub.Clients.All.SendAsync("HDDCreated", id);
            await Task.WhenAll(signalR, RedisUpdate);
            
            return id;
        }
        #endregion

        #region Remove
        public async Task RemoveAsync(Guid id)
        {
            await repo.RemoveAsync(id);
            
            var signalR = hub.Clients.All.SendAsync("HDDRemoved", id);
            var RedisUpdate = cacheService.IncrementAsync("hdd:version");

            await Task.WhenAll(signalR, RedisUpdate);
        }
        #endregion

        #region Other Opertaions
        public async ValueTask<bool> AttachHdd(Guid pcId, Guid HddId)
        {
            return await pcHddRepo.AttachHddToPc(pcId, HddId);
        }
        public async ValueTask<bool> DeattachHdd(Guid pcId, Guid HddId)
        {
            return await pcHddRepo.DeAtthachHDD(pcId, HddId);
        }

        #endregion

        #region Mapping
        private RDTOHDDCardModel Map(HDDSmallModel model) => mapper.Map<RDTOHDDCardModel>(model);
        private RDTOHDDModel Map(HDDDbModel model) => mapper.Map<RDTOHDDModel>(model);

        private HDDCreateModel Map(DTOHDDCreateModel model) => mapper.Map<HDDCreateModel>(model);
        #endregion
    }
}
