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
            await foreach (var item in context.CPUs.AsNoTracking()
                                                   .AsAsyncEnumerable()
                                                   .WithCancellation(cancellationToken))
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
            await foreach (var item in context.CPUs.AsNoTracking()
                .Select(x => new CpuSmallModel(x.ID, x.Name, x.Socket))
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async Task<IEnumerable<CpuSmallModel>> GetCpuSmallModelsPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.CPUs
           .AsNoTracking()
           .OrderBy(x => x.ID)
           .Select(x => new CpuSmallModel(x.ID, x.Name, x.Socket))
           .Skip((page - 1) * pageSize)
           .Take(pageSize)
           .ToListAsync(cancellationToken);
        }

        public async IAsyncEnumerable<CpuSmallModel> SearchCpuSmallByNameAsyncEnumerable(string name, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var model in context.CPUs.AsNoTracking()
                .Where(x => x.Name.Contains(name))
                .Select(x => new CpuSmallModel(x.ID, x.Name, x.Socket))
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken))
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
            await foreach (var model in context.CPUs.AsNoTracking()
                                                    .Where(x => x.Name == name)
                                                    .Select(x => new CpuSmallModel(x.ID, x.Name, x.Socket))
                                                    .Skip((page - 1) * pageSize)
                                                    .Take(pageSize)
                                                    .AsAsyncEnumerable()
                                                    .WithCancellation(cancellationToken))
            {
                yield return model;
            }
        }
        #endregion

        public async Task<CpuDbModel?> UpdateAsync(Guid id, CpuUpdateModel model)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var old = await context.CPUs.FirstOrDefaultAsync(x => x.ID == id);
                if (old is null) return null;
                old.Frequency = model.Frequency == 0 ? old.Frequency : model.Frequency;
                old.IsLive = model.IsLive;
                old.Count_Cores = model.Count_Cores != 0 ? model.Count_Cores : old.Count_Cores;
                old.Socket = (model.Socket is not null || model.Socket == "skip") ? model.Socket : old.Socket;
                old.Name = (model.Name is not null || model.Name == "skip") ? model.Name : old.Name;

                var payload = new PayloadKafka(
                        "CPUUpdated",
                        old.ID,
                        old.Name,
                        DateTimeOffset.UtcNow
                    ).Convert2Json();

                var outmess = new OutboxMessage
                {
                    Payload = payload,
                    EventType = "CPUUpdated",
                    AggregateId = old.ID.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    Topic = "cpu-topic"
                };
                await context.OutboxMessages.AddAsync(outmess);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return old;
            }
            catch(Exception ex) {
                await transaction.RollbackAsync();
                throw new UpdateOperationException<CpuDbModel>(ex.Message,ex);
            }
        }

        public async ValueTask<Guid> CreateAsync(CpuCreateModel model)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                CpuDbModel cpuDb = new();
                cpuDb.Frequency = model.Frequency;
                cpuDb.IsLive = model.IsLive;
                cpuDb.Count_Cores = model.Count_Cores;
                cpuDb.Socket = model.Socket;
                cpuDb.Name = model.Name;
                cpuDb.ID = Guid.NewGuid();

                var payload = new PayloadKafka(
                        "PCCreated",
                        cpuDb.ID,
                        cpuDb.Name,
                        DateTimeOffset.UtcNow
                    ).Convert2Json();

                var kafkaMess = new OutboxMessage {
                    EventType = "PCCreated",
                    Topic = "cpu-topic",
                    Payload = payload,
                    CreatedAt = DateTime.UtcNow,
                    AggregateId = cpuDb.ID.ToString()
                };
                await context.OutboxMessages.AddAsync(kafkaMess);
                await context.CPUs.AddAsync(cpuDb);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return cpuDb.ID;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new CreateOperationException<CpuDbModel>(ex.Message, ex);
            }
        }

        public async ValueTask<bool> RemoveAsync(Guid id)
        {
            CpuDbModel? removedmodel = await context.CPUs.FirstOrDefaultAsync(x => x.ID == id);
            if (removedmodel is null) return false;
            await using var transaction = await context.Database.BeginTransactionAsync();

            var payload = new PayloadKafka(
                    "CPURemoved",
                    id,
                    removedmodel.Name,
                    DateTimeOffset.UtcNow
                ).Convert2Json();

            var kafkaMessage = new OutboxMessage {
                EventType = "PCCreated",
                Topic = "cpu-topic",
                Payload = payload,
                CreatedAt = DateTime.UtcNow,
                AggregateId = id.ToString()
            };

            try
            {
                await context.OutboxMessages.AddAsync(kafkaMessage);
                context.CPUs.Remove(removedmodel);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new RemoveOperationException<CpuDbModel>($"Item CPU {id} not removed",ex);
            }
            return true;
        }


    }
}
