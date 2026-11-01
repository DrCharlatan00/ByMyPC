using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Exceptions;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ByMyPc.Postgresql.Repository
{
    public class GPURepo(PgContext context) : IGPURepo
    {
        private readonly PgContext context = context;

        #region Gets

        public async IAsyncEnumerable<GPUSmallModel> GetSmallModelsDbAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in context.GPUs.AsNoTracking()
                .Select(x => new GPUSmallModel(x.ID, x.Name, x.VideoMemorySize))
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async IAsyncEnumerable<GpuDbModel> GetFullModelsDbAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in context.GPUs.AsNoTracking()
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async Task<IEnumerable<GpuDbModel>> GetFullWithPag(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.GPUs.AsNoTracking()
                .OrderBy(x => x.ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<GpuDbModel?> GetByID(Guid id)
        {
            var item = await context.GPUs.FirstOrDefaultAsync(x => x.ID == id);
            return item;
        }

        public async Task<IEnumerable<GPUSmallModel>> GetSmallModelWithPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.GPUs.AsNoTracking()
                .OrderBy(x => x.ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GPUSmallModel(x.ID, x.Name, x.VideoMemorySize))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<GpuDbModel>?> SearchByName(string name, CancellationToken cancellationToken)
        {
            return await context.GPUs.AsNoTracking().Where(x => x.Name.Contains(name)).OrderBy(x => x.ID).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<GPUSmallModel>?> SearchByNameWithPag(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.GPUs.AsNoTracking()
                                     .Where(x => x.Name.Contains(name))
                                     .OrderBy(x => x.ID)
                                     .Select(x => new GPUSmallModel(x.ID, x.Name, x.VideoMemorySize))
                                     .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<GpuDbModel>> GetByFilter(GPUFilterModel filterModel, CancellationToken cancellationToken)
        {
            IQueryable<GpuDbModel> query = context.GPUs.AsNoTracking();

            if (filterModel.Name is not null) query = query.Where(x => x.Name.Contains(filterModel.Name));

            if (filterModel.TypeConnector is not null) query = query.Where(x => x.TypeConnector == filterModel.TypeConnector);

            if (filterModel.VideoMemorySize is not null) query = query.Where(x => x.VideoMemorySize == filterModel.VideoMemorySize);

            if (filterModel.VideoSlot is not null) query = query.Where(x => x.VideoSlot == filterModel.VideoSlot);

            if (filterModel.MemoryBus is not null) query = query.Where(x => x.MemoryBus == filterModel.MemoryBus);

            if (filterModel.TypeMemory is not null) query = query.Where(x => x.TypeMemory == filterModel.TypeMemory);

            return await query.ToListAsync();
        }


        public async Task<IEnumerable<GPUSmallModel>> GetSmallByFilter(GPUFilterModel filterModel, int page, int pageSize, CancellationToken cancellationToken)
        {
            IQueryable<GpuDbModel> query = context.GPUs.AsNoTracking();

            if (filterModel.Name is not null) query = query.Where(x => x.Name.Contains(filterModel.Name));

            if (filterModel.TypeConnector is not null) query = query.Where(x => x.TypeConnector == filterModel.TypeConnector);

            if (filterModel.VideoMemorySize is not null) query = query.Where(x => x.VideoMemorySize == filterModel.VideoMemorySize);

            if (filterModel.VideoSlot is not null) query = query.Where(x => x.VideoSlot == filterModel.VideoSlot);

            if (filterModel.MemoryBus is not null) query = query.Where(x => x.MemoryBus == filterModel.MemoryBus);

            if (filterModel.TypeMemory is not null) query = query.Where(x => x.TypeMemory == filterModel.TypeMemory);

            return await query
                .OrderBy(x => x.ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GPUSmallModel(x.ID, x.Name, x.VideoMemorySize))
                .ToListAsync(cancellationToken);
        }

        #endregion

        #region Update
        public async Task<GpuDbModel?> UpdateAsync(GPUUpdateModel model)
        {
            var old = await context.GPUs.FirstOrDefaultAsync(x => x.ID == model.ID);

            if (old is null) return null;

            await using var transaction = await context.Database.BeginTransactionAsync();
            old.Name = model.Name ?? old.Name;
            old.VideoMemorySize = model.VideoMemorySize ?? old.VideoMemorySize;
            old.VideoSlot = model.VideoSlot ?? old.VideoSlot;
            old.MemoryBus = model.MemoryBus ?? old.MemoryBus;
            old.TypeConnector = model.TypeConnector ?? old.TypeConnector;
            old.TypeMemory = model.TypeMemory ?? old.TypeMemory;

            var payload = new PayloadKafka(
                    eventType: "GPUUpdated",
                    componentID: old.ID,
                    name: old.Name,
                    occurredAt: DateTimeOffset.UtcNow
                ).Convert2Json();

            var kafkaMessage = new OutboxMessage {
                EventType = "GPUUpdated",
                Topic = "gpu-topic",
                AggregateId = old.ID.ToString(),
                CreatedAt = DateTime.UtcNow,
                Payload = payload
                
            };
            try
            {
                await context.OutboxMessages.AddAsync(kafkaMessage);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new UpdateOperationException<GpuDbModel>("Can't Save new model", ex);
            }
            return old;

        }

        #endregion

        #region Create
        public async ValueTask<(Guid id, bool IsCreated)> CreateAsync(GPUCreateModel model, string idempontencyKey, CancellationToken cancellationToken)
        {

            const string operation = "GPUCreate";

            var requestHash = Convert.ToHexString(
                SHA256.HashData(
                    JsonSerializer.SerializeToUtf8Bytes(model)
                    )
                );

            var isExisting = await context.idempotencyRecords.AsNoTracking().SingleOrDefaultAsync(x => x.Operation == operation
                                                                                                       && x.Key == idempontencyKey,
                                                                                                       cancellationToken);

            if (isExisting is not null) return (GetPreviousResult(isExisting, requestHash),false);



            GpuDbModel gpuDbModel = new GpuDbModel
            {
                ID = Guid.NewGuid(),
                Name = model.Name,
                MemoryBus = model.MemoryBus,
                TypeConnector = model.TypeConnector,
                TypeMemory = model.TypeMemory,
                VideoMemorySize = model.VideoMemorySize,
                VideoSlot = model.VideoSlot,
            };

            var payload = new PayloadKafka(
                    "GPUCreated",
                    gpuDbModel.ID,
                    gpuDbModel.Name,
                    DateTimeOffset.UtcNow
                ).Convert2Json();

            var kafkaMessage = new OutboxMessage {
                EventType = "GPUCreated",
                AggregateId = gpuDbModel.ID.ToString(),
                CreatedAt = DateTime.UtcNow,
                Topic = "gpu-topic",
                Payload = payload
            };

            var idempontencyKey2Db = new IdempotencyRecord {
                    CreateAt = DateTime.UtcNow,
                    ID = Guid.NewGuid(),
                    Key = idempontencyKey,
                    RequestHash = requestHash,
                    ResourceId = gpuDbModel.ID,
                    Operation = operation

            };
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await context.idempotencyRecords.AddAsync(idempontencyKey2Db);
                await context.GPUs.AddAsync(gpuDbModel);
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
                        x => x.Operation == operation && x.Key == idempontencyKey, cancellationToken
                    );

                if (conrurrentResult is null) throw;

                return (GetPreviousResult(conrurrentResult, requestHash), false);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new CreateOperationException<GpuDbModel>("Can't save new model", ex);
            }
            return (gpuDbModel.ID, true);
        }
        #endregion


        #region Remove 
        public async ValueTask<bool> RemoveAsync(Guid id)
        {
            var item = await context.GPUs.FirstOrDefaultAsync(x => x.ID == id);
            if (item is null) return false;

            var payload = new PayloadKafka(
        "GPURemoved",
        item.ID,
        item.Name,
        DateTimeOffset.UtcNow
    ).Convert2Json();

            var kafkaMessage = new OutboxMessage
            {
                EventType = "GPURemoved",
                AggregateId = item.ID.ToString(),
                CreatedAt = DateTime.UtcNow,
                Topic = "gpu-topic",
                Payload = payload
            };
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                context.GPUs.Remove(item);
                await context.OutboxMessages.AddAsync(kafkaMessage);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new RemoveOperationException<GpuDbModel>("Item not removed", ex);
            }
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
