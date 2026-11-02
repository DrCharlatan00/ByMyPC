using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Exceptions;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text.Json;


namespace ByMyPc.Postgresql.Repository
{
    public class RamRepo(PgContext context) : IRamRepo
    {
        private readonly PgContext context = context;

        #region Get Methods
        public async IAsyncEnumerable<RamDbModel> GetFullAsyncEnumerable([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in context.RAMs.AsNoTracking()
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async IAsyncEnumerable<RamSmallModel> GetSmallAsyncEnumerable([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (RamSmallModel? item in context.RAMs.AsNoTracking()
                .Select(x => new RamSmallModel(x.ID, x.Name, x.Size, x.DDRType))
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async Task<RamDbModel?> GetByIdAsync(Guid id)
        {
            return await context.RAMs.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<IEnumerable<RamDbModel>> GetFullWithPagAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.RAMs.AsNoTracking()
                                     .OrderBy(x => x.ID)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<RamSmallModel>> GetSmallWithPagAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.RAMs.AsNoTracking()
                                     .OrderBy(x => x.ID)
                                     .Select(x => new RamSmallModel(x.ID, x.Name, x.Size, x.DDRType))
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<RamDbModel>> SearchByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await context.RAMs.AsNoTracking()
                .OrderBy(x => x.ID)
                .Where(x => x.Name.Contains(name))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<RamSmallModel>> SearchByNameWithPagAsync(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.RAMs.AsNoTracking()
                .OrderBy(x => x.ID)
                .Where(x => x.Name.Contains(name))
                .Select(x => new RamSmallModel(x.ID, x.Name, x.Size, x.DDRType))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<RamDbModel>> GetByFilterAsync(RamFilterModel filterModel, CancellationToken cancellationToken)
        {
            IQueryable<RamDbModel> query = context.RAMs.AsNoTracking();

            if (filterModel.Name is not null) query = query.Where(x => x.Name.Contains(filterModel.Name));

            if (filterModel.Size is not null) query = query.Where(x => x.Size == filterModel.Size);

            if (filterModel.DDRType is not null) query = query.Where(x => x.DDRType == filterModel.DDRType);

            if (filterModel.Frequency is not null) query = query.Where(x => x.Frequency == filterModel.Frequency);

            if (filterModel.IsLive is not null) query = query.Where(x => x.IsLive == filterModel.IsLive);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<RamSmallModel>> GetByFilterWithPagAsync(RamFilterModel filterModel, int page, int pageSize, CancellationToken cancellationToken)
        {
            IQueryable<RamDbModel> query = context.RAMs.AsNoTracking();

            if (filterModel.Name is not null) query = query.Where(x => x.Name.Contains(filterModel.Name));

            if (filterModel.Size is not null) query = query.Where(x => x.Size == filterModel.Size);

            if (filterModel.DDRType is not null) query = query.Where(x => x.DDRType == filterModel.DDRType);

            if (filterModel.Frequency is not null) query = query.Where(x => x.Frequency == filterModel.Frequency);

            if (filterModel.IsLive is not null) query = query.Where(x => x.IsLive == filterModel.IsLive);

            return await query
                .OrderBy(x => x.ID)
                .Select(x => new RamSmallModel(x.ID, x.Name, x.Size, x.DDRType))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
        #endregion

        #region Update
        public async Task<RamDbModel?> UpdateAsync(RamUpdateModel model, string idempotencyKey, CancellationToken cancellationToken)
        {
            const string operation = "RAMUpdate";

            var requestHash = Convert.ToHexString(
                    SHA256.HashData(
                        JsonSerializer.SerializeToUtf8Bytes(model)
                        )
                );

            var isExist = await context.idempotencyRecords.AsNoTracking().SingleOrDefaultAsync(x => x.Operation == operation && x.Key == idempotencyKey,
                                                                                               cancellationToken);

            if (isExist is not null) return null;


            var old = await context.RAMs.FirstOrDefaultAsync(x => x.ID == model.ID);
            if (old is null) return null;

            try
            {
                old.Name = model.Name ?? old.Name;
                old.Size = model.Size ?? old.Size;
                old.DDRType = model.DDRType ?? old.DDRType;
                old.Frequency = model.Frequency ?? old.Frequency;
                old.IsLive = model.IsLive ?? old.IsLive;
            }
            catch (Exception ex)
            {
                throw new UpdateOperationException<RamUpdateModel>("Wrong Model", ex);
            }
            var payload = new PayloadKafka("RAMUpdated",
                   componentID: old.ID,
                   name: old.Name,
                   occurredAt: DateTimeOffset.UtcNow).Convert2Json();

            var kafkaMessage = new OutboxMessage
            {
                EventType = "RAMUpdated",
                Topic = "ram-topic",
                AggregateId = old.ID.ToString(),
                CreatedAt = DateTime.UtcNow,
                Payload = payload
            };

            var idempontencyKey2Db = new IdempotencyRecord
            {
                CreateAt = DateTime.UtcNow,
                ID = Guid.NewGuid(),
                Operation = operation,
                RequestHash = requestHash,
                ResourceId = model.ID,
                Key = idempotencyKey
            };

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await context.idempotencyRecords.AddAsync(idempontencyKey2Db);
                await context.OutboxMessages.AddAsync(kafkaMessage);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException // With Conrurrent
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
            {
                await transaction.RollbackAsync();
                context.ChangeTracker.Clear();
                var conrurrentResult = await context.idempotencyRecords.AsNoTracking().SingleOrDefaultAsync(
                        x => x.Operation == operation && x.Key == idempotencyKey, cancellationToken
                    );

                if (conrurrentResult is null) throw;
                return null;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new UpdateOperationException<RamDbModel>("Updating fail", ex); ;
            }
            return old;
        }
        #endregion

        #region Create
        public async ValueTask<(Guid,bool)> CreateAsync(RamCreateModel model, string idempotencyKey, CancellationToken cancellationToken)
        {
            const string operation = "RAMCreate";

            var requestHash = Convert.ToHexString(
                SHA256.HashData(
                    JsonSerializer.SerializeToUtf8Bytes(model)
                    )
                );

            var isExisting = await context.idempotencyRecords.AsNoTracking().SingleOrDefaultAsync(x => x.Operation == operation
                                                                                                       && x.Key == idempotencyKey,
                                                                                                       cancellationToken);

            if (isExisting is not null) return (GetPreviousResult(isExisting, requestHash),false);

            RamDbModel ramDb = new RamDbModel
            {
                ID = Guid.NewGuid(),
                Name = model.Name,
                Size = model.Size,
                DDRType = model.DDRType,
                Frequency = model.Frequency,
                IsLive = model.IsLive,
            };

            var payload = new PayloadKafka("RAMCreated",
                                           componentID: ramDb.ID,
                                           name: ramDb.Name,
                                           occurredAt: DateTimeOffset.UtcNow).Convert2Json();

            var kafkaMessage = new OutboxMessage
            {
                EventType = "RAMCreated",
                Topic = "ram-topic",
                AggregateId = ramDb.ID.ToString(),
                CreatedAt = DateTime.UtcNow,
                Payload = payload
            };

            var idempontencyKey2Db = new IdempotencyRecord
            {
                CreateAt = DateTime.UtcNow,
                ID = Guid.NewGuid(),
                Key = idempotencyKey,
                RequestHash = requestHash,
                ResourceId = ramDb.ID,
                Operation = operation

            };

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await context.idempotencyRecords.AddAsync(idempontencyKey2Db);
                await context.RAMs.AddAsync(ramDb);
                await context.OutboxMessages.AddAsync(kafkaMessage);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException // With Conrurrent
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
            {
                await transaction.RollbackAsync();
                context.ChangeTracker.Clear();

                var conrurrentResult = await context.idempotencyRecords.AsNoTracking().SingleOrDefaultAsync(
                        x => x.Operation == operation && x.Key == idempotencyKey, cancellationToken
                    );

                if (conrurrentResult is null) throw;

                return (GetPreviousResult(conrurrentResult, requestHash),false);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new CreateOperationException<RamDbModel>("Create is fail", ex);
            }
            return (ramDb.ID,true);
        }
        #endregion

        #region Remove
        public async ValueTask<bool> RemoveAsync(Guid id)
        {
            var rem = await context.RAMs.FirstOrDefaultAsync(x => x.ID == id);
            if (rem == null) return false;
            var payload = new PayloadKafka("RAMRemoved",
                               componentID: rem.ID,
                               name: rem.Name,
                               occurredAt: DateTimeOffset.UtcNow).Convert2Json();

            var kafkaMessage = new OutboxMessage
            {
                EventType = "RAMRemoved",
                Topic = "ram-topic",
                AggregateId = rem.ID.ToString(),
                CreatedAt = DateTime.UtcNow,
                Payload = payload
            };
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                context.RAMs.Remove(rem);
                await context.OutboxMessages.AddAsync(kafkaMessage);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new RemoveOperationException<RamDbModel>("Remove is failed", ex);
            }
            return true;
        }
        #endregion

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
