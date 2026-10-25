using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Caching;
using ByMyPC.Hubs;
using ByMyPC.Models;
using ByMyPC.Models.CpuModels.RDTO;
using ByMyPC.Models.GPUModels.RDTO;
using ByMyPC.Models.HDDModels.RDTO;
using ByMyPC.Models.MotherbordModels.RDTO;
using ByMyPC.Models.PCModels.DTO;
using ByMyPC.Models.PCModels.RDTO;
using ByMyPC.Models.PSUModels.RDTO;
using ByMyPC.Models.RAMModels.RDTO;
using ByMyPC.Services.Kafka;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.InteropServices;
using System.Text.Json;


namespace ByMyPC.Services.PCService
{
    public class PcService
        (
        IPcRepo repo,
        IMapper mapper,
        IValidator<DTOPCCreateModel> validator,
        ILogger<PcService> logger,
        IHubContext<PcHub> hub,
        ICacheService cacheService,
        IKafkaProducerService kafkaProducerService
        ) : IPcService
    {
        const string keyVersion = "pc:version";

        private readonly IPcRepo repo = repo;
        private readonly IMapper mapper = mapper;
        private readonly IValidator<DTOPCCreateModel> validator = validator;
        private readonly ILogger<PcService> logger = logger;
        private readonly IHubContext<PcHub> hub = hub;
        private readonly ICacheService cacheService = cacheService;
        private readonly IKafkaProducerService kafkaProducerService = kafkaProducerService;

        #region Get Methods
        public async Task<IEnumerable<RDTOPCModel>> GetFullAsync(CancellationToken cancellationToken)
        {
            ICollection<RDTOPCModel> result = new List<RDTOPCModel>();

#if DEBUG
            logger.LogInformation("Start get collection on db and mapping...........");
#endif


            await foreach (var item in repo.GetFullAsync(cancellationToken))
            {
                result.Add(Map(item));
            }

#if DEBUG
            logger.LogInformation("End Db Procces");
#endif


            return result;
        }

        public async Task<IEnumerable<RDTOPCSmallModel>> GetSmallAsync(CancellationToken cancellationToken)
        {
            ICollection<RDTOPCSmallModel> result = new List<RDTOPCSmallModel>();
#if DEBUG
            logger.LogInformation("Start get collection on db and mapping...........");
#endif
            await foreach (var item in repo.GetSmallModelsAsync(cancellationToken))
            {
                result.Add(Map(item));
            }
#if DEBUG
            logger.LogInformation("End Db Procces");
#endif
            return result;
        }

        public async Task<RDTOPCModel?> GetByIdAsync(Guid id)
        {

#if DEBUG
            logger.LogInformation("Start Get object in db by id = {id}", id);
#endif


            var item = await repo.GetByID(id);
#if DEBUG
            logger.LogInformation("End Db Procces, result = {status}", item is not null ? true : false);
#endif
            return item is not null ? Map(item) : null;
        }

        public async Task<IEnumerable<RDTOPCModel>?> GetFullPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            CheckParam(page, pageSize);

#if DEBUG
            logger.LogInformation("CheckParam pass, param  page = {page} pageSize = {pageSize}", page, pageSize);
#endif


#if DEBUG
            logger.LogInformation("Try get in cache..........");
#endif
            long version = await cacheService.GetVersionAsync(keyVersion);

            string key = $"pc:v{version}:page={page}:pageSize={pageSize}";
            var cache = await cacheService.GetAsync<IEnumerable<RDTOPCModel>>(key);
            if (cache is not null) return cache;


#if DEBUG
            logger.LogInformation("cache miss");
#endif


#if DEBUG
            logger.LogInformation("Start get Full Paginated in db................");
#endif


            var data = await repo.GetPcFullWithPagination(page, pageSize, cancellationToken);

#if DEBUG
            logger.LogInformation("End Procces in db, mapping and return, state data = {dataIsNULL}", data is not null ? true : false);
#endif
            if (data is null) return Enumerable.Empty<RDTOPCModel>() ;
            List<RDTOPCModel> mappedData = data.Select(Map).ToList();
            await cacheService.SetAsync(key, mappedData, TimeSpan.FromMinutes(3));
            return mappedData;
        }

        public async Task<IEnumerable<RDTOPCSmallModel>?> GetSmallPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            CheckParam(page, pageSize);
#if DEBUG
            logger.LogInformation("CheckParam pass, param  page = {page} pageSize = {pageSize}", page, pageSize);
#endif
            long version = await cacheService.GetVersionAsync(keyVersion);

            string key = $"pc:v{version}:page={page}:pageSize={pageSize}:small";
            var cache = await cacheService.GetAsync<IEnumerable<RDTOPCSmallModel>>(key);
            if (cache is not null) return cache;

#if DEBUG
            logger.LogInformation("Start get Small Paginated in db................");
#endif
            var data = await repo.GetPcSmallWithPagination(page, pageSize, cancellationToken);
#if DEBUG
            logger.LogInformation("End Procces in db, mapping and return, state data = {dataIsNULL}", data is not null ? true : false);
#endif
            if (data is null) return Enumerable.Empty<RDTOPCSmallModel>();
            List<RDTOPCSmallModel> mappedData = data.Select(Map).ToList();
            await cacheService.SetAsync(key, mappedData, TimeSpan.FromMinutes(3));
            return mappedData;
        }

        public async Task<IEnumerable<RDTOPCModel>?> SearchByNameAsync(string name, CancellationToken cancellationToken)
        {
            CheckParam(name);

#if DEBUG
            logger.LogInformation("CheckParam pass, param name = {name}", name);
#endif

#if DEBUG
            logger.LogInformation("try get in cache....");
#endif
            long version = await cacheService.GetVersionAsync(keyVersion);

            string key = $"pc:v{version}:name={name}";

            var cache = await cacheService.GetAsync<IEnumerable<RDTOPCModel>>(key);
            if (cache is not null) return cache;


#if DEBUG
            logger.LogInformation("cache is miss");
#endif



#if DEBUG
            logger.LogInformation("Start Search by name in db....................");
#endif


            var data = await repo.SearchByName(name, cancellationToken);

#if DEBUG
            logger.LogInformation("End Procces in db, mapping and return, state data = {dataIsNULL}", data is not null ? true : false);
#endif

            if (data is null) return Enumerable.Empty<RDTOPCModel>();

#if DEBUG
            logger.LogInformation("Mapping...");
#endif


            var mapped = data.Select(Map).ToList();


#if DEBUG
            logger.LogInformation("Caching....");
#endif
            await cacheService.SetAsync(key,mapped,TimeSpan.FromMinutes(3));

            return mapped;

        }

        public async Task<IEnumerable<RDTOPCSmallModel>?> SearchByNamePaginatedAsync(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            CheckParam(page, pageSize, name);

#if DEBUG
            logger.LogInformation("CheckParam pass, param  page = {page} pageSize = {pageSize} name = {name}", page, pageSize, name);
#endif

#if DEBUG
            logger.LogInformation("try get in cache....");
#endif
            long version = await cacheService.GetVersionAsync(keyVersion);

            string key = $"pc:v{version}:name={name}:page={page}:pageSize={pageSize}";

            var cache = await cacheService.GetAsync<IEnumerable<RDTOPCSmallModel>>(key);
            if (cache is not null) return cache;


#if DEBUG
            logger.LogInformation("cache is miss");
#endif

#if DEBUG
            logger.LogInformation("Start Search by name in db....................");
#endif

            var data = await repo.SearchByNameWithPag(name, page, pageSize, cancellationToken);

#if DEBUG
            logger.LogInformation("End Procces in db, mapping and return, state data = {dataIsNULL}", data is not null ? true : false);
#endif
            if (data is null) return Enumerable.Empty<RDTOPCSmallModel>();

#if DEBUG
            logger.LogInformation("Mapping...");
#endif


            var mapped = data.Select(Map).ToList();


#if DEBUG
            logger.LogInformation("Caching....");
#endif
            await cacheService.SetAsync(key, mapped, TimeSpan.FromMinutes(3));

            return mapped;

        }

        public async Task<IEnumerable<RDTOPCModel>?> GetByFilterAsync(DTOPCFilter filter, CancellationToken cancellationToken)
        {

#if DEBUG
            logger.LogInformation("Mapping filter = {@filter} to Db format..........", filter);
#endif


            var filter2Db = Map(filter);

#if DEBUG
            logger.LogInformation("End Mapping, Start Filtering in Db...............");
#endif


            var data = await repo.GetByFilter(filter2Db, cancellationToken);
#if DEBUG
            logger.LogInformation("End Procces in db, mapping and return, state data = {dataIsNULL}", data is not null ? true : false);
#endif
            return data is not null ? data.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOPCSmallModel>?> GetByFilterAsyncPaginated(DTOPCFilter filter, int page, int pageSize, CancellationToken cancellationToken)
        {

            CheckParam(page, pageSize);
#if DEBUG
            logger.LogInformation("CheckParam pass, param  page = {page} pageSize = {pageSize} ", page, pageSize);
#endif
#if DEBUG
            logger.LogInformation("Mapping filter = {@filter} to Db format..........", filter);
#endif
            var filter2Db = Map(filter);
#if DEBUG
            logger.LogInformation("End Mapping, Start Filtering in Db...............");
#endif
            var data = await repo.GetByFilterWithPagination(filter2Db, page, pageSize, cancellationToken);
#if DEBUG
            logger.LogInformation("End Procces in db, mapping and return, state data = {dataIsNULL}", data is not null ? true : false);
#endif
            return data is not null ? data.Select(Map).ToList() : null;
        }
        #endregion

        #region Update
        public async Task<RDTOPCModel?> UpdateAsync(DTOPCUpdateModel updateModel)
        {

#if DEBUG
            logger.LogInformation("Update model is = {@updateModel}", updateModel);
#endif


            PcUpdateModel model = new PcUpdateModel(updateModel.id, updateModel.name);

#if DEBUG
            logger.LogInformation("Start Updating in db......");
#endif


            PcDbModel? updatedModel = await repo.UpdateAsync(model);

#if DEBUG
            logger.LogInformation("End Db Procces, Mapping model ");
#endif


            RDTOPCModel? mappedModel = updatedModel is not null ? Map(updatedModel!) : null;
            if (mappedModel is null) return null;

            var hubWork = hub.Clients.All.SendAsync("PcUpdated",updateModel.id);
            var cacheWork = cacheService.IncrementAsync(keyVersion);

#if DEBUG
            logger.LogInformation("Start Side Services");
#endif


            await Task.WhenAll(hubWork, cacheWork);
#if DEBUG
            logger.LogInformation("Check and Mapping end. Result = {@res}", mappedModel);
#endif

            return mappedModel;

        }

        #endregion

        #region Create
        public async ValueTask<Guid> CreateAsync(DTOPCCreateModel createModel)
        {

#if DEBUG
            logger.LogInformation("Service start Creating new object. DTO = {@DTO}", createModel);
#endif


#if DEBUG
            logger.LogInformation("Validate..............");
#endif


            await validator.ValidateAndThrowAsync(createModel);




            var model = new PcCreateModel(createModel.name);

#if DEBUG
            logger.LogInformation("Validator pass data, Start Create in Db..................");
#endif

            Guid res = await repo.CreateAsync(model);
#if DEBUG
            logger.LogInformation("Creating end");
#endif
            var hubWork = hub.Clients.All.SendAsync("PcCreated", res);
            var cacheWork = cacheService.IncrementAsync(keyVersion);


#if DEBUG
            logger.LogInformation("Start Side Services");
#endif


            await Task.WhenAll(hubWork, cacheWork);

            return res;
        }

        #endregion

        #region Remove 

        public async ValueTask<bool> RemoveAsync(Guid id)
        {

#if DEBUG
            logger.LogInformation("Start Removing in Db..............");
#endif


            bool res = await repo.RemoveAsync(id);

#if DEBUG
            logger.LogInformation("End removing, result = {result}", res);
#endif
            if (!res) return false;

            var hubWork = hub.Clients.All.SendAsync("PcRemoved", id);
            var cacheWork = cacheService.IncrementAsync(keyVersion);

#if DEBUG
            logger.LogInformation("Start Side Services");
#endif

            await Task.WhenAll(hubWork, cacheWork);

            return res;
        }

        #endregion

        #region Attach Methods
        public async ValueTask<bool> AttachAsync(Guid idPc, Guid idComponent, WithAttachOrDeattachToPc attachComponent)
        {

#if DEBUG
            logger.LogInformation("Start Attacing {whatAttach} in db.......", attachComponent.ToString());
#endif


            bool result = false;
            switch (attachComponent)
            {
                case WithAttachOrDeattachToPc.CPU:
                    result = await repo.AttachCPUAsync(idPc, idComponent);
                    break;
                case WithAttachOrDeattachToPc.GPU:
                    result = await repo.AttachGPUAsync(idPc, idComponent);
                    break;
                case WithAttachOrDeattachToPc.MOTHERBOARD:
                    result = await repo.AttachMotherboardAsync(idPc, idComponent);
                    break;
                case WithAttachOrDeattachToPc.PSU:
                    result = await repo.AttachPSUAsync(idPc, idComponent);
                    break;
                default:
                    return false;
            }

#if DEBUG
            logger.LogInformation("Attach is end, result = {res}", result);
#endif
            var hubWork = hub.Clients.All.SendAsync("PcAttached", idPc);
            var cacheWork = cacheService.IncrementAsync(keyVersion);

#if DEBUG
            logger.LogInformation("Start Side Services");
#endif


            await Task.WhenAll(hubWork, cacheWork);
            return result;
        }
        #endregion

        #region Deattach Methods
        public async ValueTask<bool> DeattachAsync(Guid idPc, Guid idComponent, WithAttachOrDeattachToPc attachComponent)
        {
#if DEBUG
            logger.LogInformation("Start DeAttacing {whatDeAttach} in db.......", attachComponent.ToString());
#endif

            bool result = false;
            switch (attachComponent)
            {
                case WithAttachOrDeattachToPc.CPU:
                    result = await repo.DeattachCPUAsync(idPc, idComponent);
                    break;
                case WithAttachOrDeattachToPc.GPU:
                    result = await repo.DeattachGPUAsync(idPc, idComponent);
                    break;
                case WithAttachOrDeattachToPc.MOTHERBOARD:
                    result = await repo.DeattachMotherbordAsync(idPc, idComponent);
                    break;
                case WithAttachOrDeattachToPc.PSU:
                    result = await repo.DeattachPSUAsync(idPc, idComponent);
                    break;
                default:
                    return false;
            }
#if DEBUG
            logger.LogInformation("Attach is end, result = {res}", result);
#endif
            var hubWork = hub.Clients.All.SendAsync("PcDettached", idPc);
            var cacheWork = cacheService.IncrementAsync(keyVersion);

#if DEBUG
            logger.LogInformation("Start Side Services");
#endif


            await Task.WhenAll(hubWork, cacheWork);
            return result;
        }
        #endregion

        #region Mappers
        private PcFilterModel Map(DTOPCFilter filter) => new PcFilterModel(
            filter.name,
            filter.countCore,
            filter.videoMemorySize,
            filter.socket,
            filter.powerWatt
            );
        private RDTOPCModel Map(PcDbModel model)
        {
            ICollection<RDTORAMSmallModel> rams = new List<RDTORAMSmallModel>();
            foreach (PcRamDbModel item in model.Rams)
            {
                if (item is null) continue;
                rams.Add(MapRAM(item.Ram));
            }

            ICollection<RDTOHDDCardModel> Hdd = new List<RDTOHDDCardModel>();

            foreach (var item in model.HDDs)
            {
                if (item is null) continue;
                Hdd.Add(MapHDD(item.Hdd));
            }

            return new RDTOPCModel(
                model.ID,
                model.Name,
                new PropertiesPc(
                    model.Cpu is not null ? MapCpu(model.Cpu) : null,
                    model.Gpu is not null ? MapGpu(model.Gpu) : null,
                    model.Motherboard is not null ? MapMb(model.Motherboard) : null,
                    model.PSU is not null ? MapPSU(model.PSU) : null,
                    rams,
                    Hdd
                    )
            );
        }

        private RDTOPCSmallModel Map(PcSmallModel model)
        {
            return new RDTOPCSmallModel(
            model.ID, model.Name,
            new PropertiesPcSmall(model.CpuId, model.GpuId, model.MotherboardId, model.PSUId, model.Rams, model.Hdd)
            );
        }

        //private RDTOCpuModel MapCpu(CpuDbModel model) => mapper.Map<RDTOCpuModel>(model);
        private RDTOCpuSmallModel MapCpu(CpuSmallModel model) => mapper.Map<RDTOCpuSmallModel>(model);
        private RDTOCpuSmallModel MapCpu(CpuDbModel model) => mapper.Map<RDTOCpuSmallModel>(model);


        private RDTOGPUSmallModel MapGpu(GPUSmallModel model) => mapper.Map<RDTOGPUSmallModel>(model);
        private RDTOGPUSmallModel MapGpu(GpuDbModel model) => mapper.Map<RDTOGPUSmallModel>(model);


        private RDTORAMSmallModel MapRAM(RamDbModel model) => mapper.Map<RDTORAMSmallModel>(model);

        private RDTOPSUSmallModel MapPSU(PSUDbModel model) => mapper.Map<RDTOPSUSmallModel>(model);

        private RDTOModelMotherboardCard MapMb(MotherboardDbModel model) => mapper.Map<RDTOModelMotherboardCard>(model);

        private RDTOHDDCardModel MapHDD(HDDDbModel model) => mapper.Map<RDTOHDDCardModel>(model);
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
