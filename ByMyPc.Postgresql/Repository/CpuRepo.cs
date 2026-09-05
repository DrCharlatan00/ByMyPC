using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Exceptions;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ByMyPc.Postgresql.Repository
{
    public class CpuRepo(PgContext context) : ICpuRepo
    {
        private readonly PgContext context = context;

        #region Get
        public async IAsyncEnumerable<CpuDbModel> GetCpuRepoAsyncEnumerable([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in context.CPUs.AsNoTracking().AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async Task<IEnumerable<CpuDbModel>> GetAsyncPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.CPUs
                .AsNoTracking()
                .OrderBy(x => x.ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<CpuDbModel?> GetByIDAsync(Guid id)
        {
            return await context.CPUs.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
        }
        #endregion

        #region SmallModel
        public async IAsyncEnumerable<CpuSmallModel> GetCpuSmallRepoAsyncEnumerable([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in context.CPUs.AsNoTracking().Select(x => new CpuSmallModel(x.Name, x.Socket)).AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async Task<IEnumerable<CpuSmallModel>> GetCpuSmallModelsPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.CPUs
           .AsNoTracking()
           .OrderBy(x => x.ID)
           .Select(x => new CpuSmallModel(x.Name, x.Socket))
           .Skip((page - 1) * pageSize)
           .Take(pageSize)
           .ToListAsync(cancellationToken);
        }

        public async IAsyncEnumerable<CpuSmallModel> SearchCpuSmallByNameAsyncEnumerable(string name, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var model in context.CPUs.AsNoTracking().Where(x => x.Name == name).Select(x => new CpuSmallModel(x.Name, x.Socket)).AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return model;
            }
        }

        public async Task<IEnumerable<CpuDbModel>> GetByFilterAsync(CPUFilterModel filterModel,CancellationToken cancellationToken) {

            IQueryable<CpuDbModel> query = context.CPUs.AsNoTracking();

            if (filterModel.ByName is not null)
                query = query.Where(x => x.Name.Contains(filterModel.ByName));

            if (filterModel.ByQuantityCores is not null) query = query.Where(x => x.Count_Cores == filterModel.ByQuantityCores);

            if (filterModel.ByLive is not null) {
                query = query.Where(x => x.IsLive == filterModel.ByLive); 
            }
            return await query.ToListAsync(cancellationToken);
        }


        public async Task<IEnumerable<CpuDbModel>> GetByFilterWithPagAsync(CPUFilterModel filterModel,int page,int pageSize, CancellationToken cancellationToken)
        {

            IQueryable<CpuDbModel> query = context.CPUs.AsNoTracking();

            if (filterModel.ByName is not null)
                query = query.Where(x => x.Name.Contains(filterModel.ByName));

            if (filterModel.ByQuantityCores is not null) query = query.Where(x => x.Count_Cores == filterModel.ByQuantityCores);

            if (filterModel.ByLive is not null)
                query = query.Where(x => x.IsLive == filterModel.ByLive);
            
            return await query.Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync(cancellationToken);
        }



        public async IAsyncEnumerable<CpuSmallModel> SearchCpuSmallByNameWithPaginationAsyncEnumerable(string name, int page, int pageSize, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var model in context.CPUs.AsNoTracking().Where(x => x.Name == name).Select(x => new CpuSmallModel(x.Name, x.Socket)).Skip((page - 1) * pageSize).Take(pageSize).AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return model;
            }
        }
        #endregion

        public async Task<CpuDbModel?> UpdateAsync(Guid id, CpuUpdateModel model)
        {
            try
            {
                var old = await context.CPUs.FirstOrDefaultAsync(x => x.ID == id);
                if (old is null) return null;
                old.Frequency = model.Frequency == 0 ? old.Frequency : model.Frequency;
                old.IsLive = model.IsLive;
                old.Count_Cores = model.Count_Cores != 0 ? model.Count_Cores : old.Count_Cores;
                old.Socket = (model.Socket is not null || model.Socket == "skip") ? model.Socket : old.Socket;
                old.Name = (model.Name is not null || model.Name == "skip") ? model.Name : old.Name;
                await context.SaveChangesAsync();
                return old;
            }
            catch(Exception ex) {
                throw new UpdateOperationException<CpuDbModel>(ex.Message,ex);
            }
        }

        public async Task<Guid> CreateAsync(CpuCreateModel model)
        {
            try
            {
                CpuDbModel cpuDb = new();
                cpuDb.Frequency = model.Frequency;
                cpuDb.IsLive = model.IsLive;
                cpuDb.Count_Cores = model.Count_Cores;
                cpuDb.Socket = model.Socket;
                cpuDb.Name = model.Name;
                cpuDb.ID = Guid.NewGuid();
                await context.CPUs.AddAsync(cpuDb);
                await context.SaveChangesAsync();
                return cpuDb.ID;
            }
            catch (Exception ex)
            {
                throw new CreateOperationException<CpuDbModel>(ex.Message, ex);
            }
        }

        public async Task RemoveAsync(Guid id)
        {
            CpuDbModel? removedmodel = await context.CPUs.FirstOrDefaultAsync(x => x.ID == id);
            if (removedmodel is null) throw new RemoveOperationException<CpuDbModel>();
            context.CPUs.Remove(removedmodel);
            await context.SaveChangesAsync();
        }


    }
}
