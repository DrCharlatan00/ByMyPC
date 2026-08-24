using AutoMapper;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Models.CpuModels.DTO;
using ByMyPC.Models.CpuModels.RDTO;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;

namespace ByMyPC.Services.CpuService
{
    public class CpuService(ICpuRepo repo,
        IValidator<DTOCpuCreateModel> validatorCreate,
        IValidator<DTOCpuUpdateModel> validatorUpdate,
        IMapper mapper,
        ILogger<CpuService> logger
        ) : ICpuService
    {
        private readonly ICpuRepo repo = repo;
        private readonly IValidator<DTOCpuCreateModel> validatorCreate = validatorCreate;
        private readonly IValidator<DTOCpuUpdateModel> validatorUpdate = validatorUpdate;
        private readonly IMapper mapper = mapper;
        private readonly ILogger<CpuService> logger = logger;

        public async Task<IEnumerable<RDTOCpuModel>> GetFullCpuAsync(CancellationToken cancellationToken)
        {
            List<RDTOCpuModel> RDTO = new();
            await foreach (var item in repo.GetCpuRepoAsyncEnumerable(cancellationToken))
            {
                RDTO.Add(Map(item));
            }
            if (RDTO is null) logger.LogWarning("Collection {collection} is db is null", nameof(IEnumerable<RDTOCpuModel>));
            return RDTO;
        }

        public async Task<IEnumerable<RDTOCpuModel>> GetFullCpuPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            var data = await repo.GetAsyncPagination(page, pageSize, cancellationToken);
            return data.Select(Map).ToList();
        }

        public async Task<RDTOCpuModel?> GetById(Guid Id)
        {
            var data = await repo.GetByIDAsync(Id);
            return data is null ? null : Map(data);
        }

        public async Task<IEnumerable<RDTOSmallModel>> GetRDTOSmallModelAsync(CancellationToken cancellationToken)
        {
            List<RDTOSmallModel> RDTO = new();
            await foreach (var item in repo.GetCpuSmallRepoAsyncEnumerable(cancellationToken))
            {
                RDTO.Add(Map(item));
            }
            return RDTO;
        }

        public async Task<IEnumerable<RDTOSmallModel>> GetSmallModelsWithPaginationAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var data = await repo.GetCpuSmallModelsPagination(page, pageSize, cancellationToken);
            return data.Select(Map).ToList();
        }

        public async Task<IEnumerable<RDTOSmallModel>> SearchByNameAsync(string name, CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("Func {func} Get : name: {name}", nameof(SearchByNameAsync), name);
#endif
            List<RDTOSmallModel> RDTO = new();
            await foreach (var data in repo.SearchCpuSmallByNameAsyncEnumerable(name, cancellationToken))
            {
                RDTO.Add(Map(data));
            }
            return RDTO;

        }

        public async Task<IEnumerable<RDTOSmallModel>> SearchByNameWithPaginationAsync(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("Func {func} Get : name: {name} , page: {page} , pageSize: {pageSize}", nameof(SearchByNameWithPaginationAsync), name, page, pageSize);
#endif
            List<RDTOSmallModel> RDTO = new();
            await foreach (var data in repo.SearchCpuSmallByNameWithPaginationAsyncEnumerable(name, page, pageSize, cancellationToken))
            {
                RDTO.Add(Map(data));
            }
            return RDTO;
        }

        public async Task<RDTOCpuModel?> UpdateAsync(DTOCpuUpdateModel model)
        {
#if DEBUG
            logger.LogInformation("Func {func} Get : model: {@model}", nameof(UpdateAsync), model);
#endif
            ArgumentNullException.ThrowIfNull(model);
            await validatorUpdate.ValidateAndThrowAsync(model);
            var result = await repo.UpdateAsync(model.id, Map(model));
            return result is null ? null : Map(result);

        }

        public async Task<Guid> CreateAsync(DTOCpuCreateModel model)
        {
#if DEBUG
            logger.LogInformation("Func {func} Get : model: {@model}", nameof(CreateAsync), model);
#endif
            ArgumentNullException.ThrowIfNull(model);
            await validatorCreate.ValidateAndThrowAsync(model);
            var result = await repo.CreateAsync(Map(model));
            return result;
        }

        public async Task RemoveAsync(Guid id)
        {
#if DEBUG
            logger.LogInformation("Func {func} Get : id: {id}", nameof(RemoveAsync), id);
#endif
            await repo.RemoveAsync(id);
        }


        private RDTOCpuModel Map(CpuDbModel model) => mapper.Map<RDTOCpuModel>(model);
        private RDTOSmallModel Map(CpuSmallModel model) => mapper.Map<RDTOSmallModel>(model);
        private CpuUpdateModel Map(DTOCpuUpdateModel model) => mapper.Map<CpuUpdateModel>(model);
        private CpuCreateModel Map(DTOCpuCreateModel model) => mapper.Map<CpuCreateModel>(model);

    }


}
