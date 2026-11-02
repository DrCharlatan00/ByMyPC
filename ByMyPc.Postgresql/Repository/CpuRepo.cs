using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Exceptions;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;

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
                                                    .Where(x => x.Name.Contains(name))
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

        public async Task<CpuDbModel?> UpdateAsync(Guid id, CpuUpdateModel model, string idempotencyKey, CancellationToken cancellationToken)
        {
            const string operation = "CPUUpdate";

            var requestHash = Convert.ToHexString(
                    SHA256.HashData(
                        JsonSerializer.SerializeToUtf8Bytes(model)
                        )
                );

            var isExist = await context.idempotencyRecords.AsNoTracking().SingleOrDefaultAsync(x => x.Operation == operation && x.Key == idempotencyKey,
                                                                                               cancellationToken);
            if (isExist is not null) return null;


            var old = await context.CPUs.FirstOrDefaultAsync(x => x.ID == id);
            if (old is null) return null;

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {

                old.Frequency = model.Frequency == 0 ? old.Frequency : model.Frequency;
                old.IsLive = model.IsLive;
                old.Count_Cores = model.Count_Cores != 0 ? model.Count_Cores : old.Count_Cores;
                old.Socket = (model.Socket is not null && model?.Socket != "skip") ? model.Socket : old.Socket;
                old.Name = (model.Name is not null && model?.Name != "skip") ? model.Name : old.Name;

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

                var idempotencyKey2Db = new IdempotencyRecord
                {
                    CreateAt = DateTime.UtcNow,
                    ID = Guid.NewGuid(),
                    Operation = operation,
                    RequestHash = requestHash,
                    ResourceId = old.ID,
                    Key = idempotencyKey
                };
                await context.idempotencyRecords.AddAsync(idempotencyKey2Db);
                await context.OutboxMessages.AddAsync(outmess);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return old;
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is PostgresException
                {
                    SqlState: PostgresErrorCodes.UniqueViolation
                }
            )
            {
                await transaction.RollbackAsync();
                context.ChangeTracker.Clear();
                var concurrentResult = await context.idempotencyRecords.AsNoTracking().SingleOrDefaultAsync(x => x.Operation == operation && x.Key == idempotencyKey,
                                                                                   cancellationToken);
                if (concurrentResult is null) throw;
                return null;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new UpdateOperationException<CpuDbModel>(ex.Message, ex);
            }
        }

        public async ValueTask<(Guid id, bool IsCreated)> CreateAsync(CpuCreateModel model, string idempotencyKey, CancellationToken cancellationToken)
        {
            const string operation = "CPUCreate";
            var requestHash = Convert.ToHexString(
                    SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(model))
                );

            var isExist = await context.idempotencyRecords.AsNoTracking().SingleOrDefaultAsync(x => x.Operation == operation
                                                                                                    && x.Key == idempotencyKey, cancellationToken: cancellationToken);

            if (isExist is not null) return (GetPreviousResult(isExist, requestHash), false);

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
                        "CPUCreated",
                        cpuDb.ID,
                        cpuDb.Name,
                        DateTimeOffset.UtcNow
                    ).Convert2Json();

                var kafkaMess = new OutboxMessage {
                    EventType = "CPUCreated",
                    Topic = "cpu-topic",
                    Payload = payload,
                    CreatedAt = DateTime.UtcNow,
                    AggregateId = cpuDb.ID.ToString()
                };

                var idempotencyKey2Db = new IdempotencyRecord {
                    CreateAt = DateTime.UtcNow,
                    ID = Guid.NewGuid(),
                    Key = idempotencyKey,
                    Operation = operation,
                    RequestHash = requestHash,
                    ResourceId = cpuDb.ID
                };
                await context.idempotencyRecords.AddAsync(idempotencyKey2Db);
                await context.OutboxMessages.AddAsync(kafkaMess);
                await context.CPUs.AddAsync(cpuDb);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (cpuDb.ID, true);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException {
                SqlState: PostgresErrorCodes.UniqueViolation
            }) 
            {
                await transaction.RollbackAsync();
                context.ChangeTracker.Clear();

                var concurrentResult = await context.idempotencyRecords.AsNoTracking().SingleOrDefaultAsync(x => x.Operation == operation
                                                                                                           && x.Key == idempotencyKey, cancellationToken: cancellationToken);
                if (concurrentResult is null) throw;
                return (GetPreviousResult(concurrentResult, requestHash), false);
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
                EventType = "CPURemoved",
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

        #region Checker
        private static Guid GetPreviousResult(
        IdempotencyRecord record,
        string requestHash)
        {
            if (!CryptographicOperations.FixedTimeEquals(
                    Convert.FromHexString(record.RequestHash),
                    Convert.FromHexString(requestHash)))
            {
                throw new IdempotencyKeyConflictException(
                    "this Idempotency-Key in use with other request.");
            }

            return record.ResourceId;
        }
        #endregion
    }
}
