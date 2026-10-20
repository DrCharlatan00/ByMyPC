using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Models.CpuModels.RDTO;
using ByMyPC.Models.GPUModels.RDTO;
using ByMyPC.Models.HDDModels.RDTO;
using ByMyPC.Models.MotherbordModels.RDTO;
using ByMyPC.Models.PCModels.DTO;
using ByMyPC.Models.PCModels.RDTO;
using ByMyPC.Models.PSUModels.RDTO;
using ByMyPC.Models.RAMModels.RDTO;
using FluentValidation;
using Npgsql;

namespace ByMyPC.Services.PCService
{
    public class PcService
        (
        IPcRepo repo,
        IMapper mapper,
        IValidator<DTOPCCreateModel> validator,
        ILogger<PcService> logger
        )
    {
        private readonly IPcRepo repo = repo;
        private readonly IMapper mapper = mapper;
        private readonly IValidator<DTOPCCreateModel> validator = validator;
        private readonly ILogger<PcService> logger = logger;

        #region Get Methods
        public async Task<IEnumerable<RDTOPCModel>> GetFullAsync(CancellationToken cancellationToken)
        {
            ICollection<RDTOPCModel> result = new List<RDTOPCModel>();
            await foreach (var item in repo.GetFullAsync(cancellationToken))
            {
                result.Add(Map(item));
            }
            return result;
        }

        public async Task<IEnumerable<RDTOPCSmallModel>> GetSmallAsycn(CancellationToken cancellationToken)
        {
            ICollection<RDTOPCSmallModel> result = new List<RDTOPCSmallModel>();
            await foreach (var item in repo.GetSmallModelsAsync(cancellationToken))
            {
                result.Add(Map(item));
            }
            return result;
        }

        public async Task<RDTOPCModel?> GetByIdAsync(Guid id)
        {
            var item = await repo.GetByID(id);
            return item is not null ? Map(item) : null;
        }

        public async Task<IEnumerable<RDTOPCModel>?> GetFullPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var data = await repo.GetPcFullWithPagination(page, pageSize, cancellationToken);
            return data is not null ? data.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOPCSmallModel>?> GetSmallPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var data = await repo.GetPcSmallWithPagination(page, pageSize, cancellationToken);
            return data is not null ? data.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOPCModel>?> SearchByNameAsync(string name, CancellationToken cancellationToken) {
            var data = await repo.SearchByName(name, cancellationToken);
            return data is not null ? data.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOPCSmallModel>?> SearchByNamePaginatedAsync(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            var data = await repo.SearchByNameWithPag(name,page,pageSize, cancellationToken);
            return data is not null ? data.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOPCModel>?> GetByFilterAsync(DTOPCFilter filter,CancellationToken cancellationToken)
        {
            var filter2Db = Map(filter);
            var data = await repo.GetByFilter(filter2Db, cancellationToken);
            return data is not null ? data.Select(Map).ToList() : null;
        }

        public async Task<IEnumerable<RDTOPCSmallModel>?> GetByFilterAsyncPaginated(DTOPCFilter filter, int page, int pageSize, CancellationToken cancellationToken)
        {
            var filter2Db = Map(filter);
            var data = await repo.GetByFilterWithPagination(filter2Db, page, pageSize, cancellationToken);
            return data is not null ? data.Select(Map).ToList() : null;
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
            new PropertiesPcSmall(model.CpuId, model.GpuId, model.MotherboardId, model.PSUId,model.Rams,model.Hdd)
            );
        }

        //private RDTOCpuModel MapCpu(CpuDbModel model) => mapper.Map<RDTOCpuModel>(model);
        private RDTOCpuSmallModel MapCpu (CpuSmallModel model) => mapper.Map<RDTOCpuSmallModel>(model);
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
