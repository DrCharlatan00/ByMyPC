using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Exceptions;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ByMyPc.Postgresql.Repository
{
    public class MotherboardRepo(PgContext context) : IMotherboardRepo
    {
        private readonly PgContext context = context;

        #region Get

        public async IAsyncEnumerable<MotherboardSmallDbModel> GetCardMotherboardDbAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in context.Motherboards.AsNoTracking()
                .Select(u => new MotherboardSmallDbModel(u.ID, u.Name, u.Socket, u.IsLive))
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }
        public async IAsyncEnumerable<MotherboardDbModel> GetFullMotherboardDbAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in context.Motherboards.AsNoTracking().AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async Task<IEnumerable<MotherboardSmallDbModel>> GetCardWithPaginationAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.Motherboards.AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(x => x.ID)
                .Select(u => new MotherboardSmallDbModel(u.ID, u.Name, u.Socket, u.IsLive))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MotherboardDbModel>> GetFullWithPag(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.Motherboards.AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(x => x.ID)
                .ToListAsync(cancellationToken);
        }

        public async Task<MotherboardDbModel?> GetByIDAsync(Guid id)
        {
            return await context.Motherboards.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<IEnumerable<MotherboardSmallDbModel>> SearchByNameMotherboardSmallAsync(string name, CancellationToken cancellationToken)
        {
            return await context.Motherboards.AsNoTracking()
                .Where(x => x.Name.Contains(name))
                .Select(u => new MotherboardSmallDbModel(u.ID, u.Name, u.Socket, u.IsLive))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MotherboardSmallDbModel>> SearchByNameMotherboardSmallWithPaginationAsync(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.Motherboards.AsNoTracking()
                .Where(x => x.Name.Contains(name))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new MotherboardSmallDbModel(u.ID, u.Name, u.Socket, u.IsLive))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MotherboardDbModel>> GetByFilterAsync(MotherBoardFilterModel filter, CancellationToken cancellationToken) {
            IQueryable<MotherboardDbModel> query = context.Motherboards.AsNoTracking();

            if (filter.ByName is not null) query = query.Where(x => x.Name.Contains(filter.ByName));

            if (filter.ByLive is not null) query = query.Where(x => x.IsLive == filter.ByLive);

            if (filter.ByHaveIntegratedGPU is not null) query = query.Where(x => x.IntegrationGpu == filter.ByHaveIntegratedGPU);

            if (filter.BySocket is not null) query = query.Where(x => x.Socket.Contains(filter.BySocket));

            return await query.ToListAsync(cancellationToken);

        }

        public async Task<IEnumerable<MotherboardSmallDbModel>> GetByFilterWithPagAsync(MotherBoardFilterModel filter,int page, int pageSize, CancellationToken cancellationToken)
        {
            IQueryable<MotherboardDbModel> query = context.Motherboards.AsNoTracking();

            if (filter.ByName is not null) query = query.Where(x => x.Name.Contains(filter.ByName));
            
            if (filter.ByLive is not null) query = query.Where(x => x.IsLive == filter.ByLive);

            if (filter.ByHaveIntegratedGPU is not null) query = query.Where(x => x.IntegrationGpu == filter.ByHaveIntegratedGPU);

            if (filter.BySocket is not null) query = query.Where(x => x.Socket.Contains(filter.BySocket));

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new MotherboardSmallDbModel(u.ID, u.Name, u.Socket, u.IsLive))
                .ToListAsync(cancellationToken);

        }
        #endregion

        #region Update


        public async Task<MotherboardDbModel?> UpdateAsync(MotherboardUpdateModel model, string idempotencyKey, CancellationToken cancellationToken)
        {

            const string opertation = "MBUpdate";

            var requestHash = Convert.ToHexString(
                    SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(idempotencyKey))
                );

            var isExist = await context.idempotencyRecords.AsNoTracking().SingleOrDefaultAsync(x => x.Operation == opertation && x.Key == idempotencyKey, cancellationToken);

            if (isExist is not null) return null;

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                MotherboardDbModel? old = await context.Motherboards.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (old is null) return null;
                old.Name = !string.IsNullOrWhiteSpace(model.Name) ? model.Name : old.Name;
                old.Socket = !string.IsNullOrWhiteSpace(model.Socket) ? model.Socket : old.Socket;
                old.IsLive = model.IsLive;

                var payload = new PayloadKafka(
                        "MBUpdated",
                        old.ID,
                        old.Name,
                        DateTimeOffset.UtcNow
                    ).Convert2Json();

                var kafkaMessage = new OutboxMessage {
                    EventType = "MBUpdated",
                    Topic = "mb-topic",
                    AggregateId = old.ID.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    Payload = payload
                };

                var idempotencyKey2Db = new IdempotencyRecord {
                    CreateAt = DateTime.UtcNow,
                    ID = Guid.NewGuid(),
                    Key = idempotencyKey,
                    Operation = opertation,
                    RequestHash = requestHash,
                    ResourceId = old.ID
                };

                await context.idempotencyRecords.AddAsync(idempotencyKey2Db);
                await context.OutboxMessages.AddAsync(kafkaMessage);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return old;
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException 
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            }) 
            {
                await transaction.RollbackAsync();
                context.ChangeTracker.Clear();

                var concurrateResult = await context.idempotencyRecords.AsNoTracking().SingleOrDefaultAsync(x => x.Operation == opertation && x.Key == idempotencyKey,
                                                                                                            cancellationToken);
                if (concurrateResult is null) throw;

                return null;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new UpdateOperationException<MotherboardDbModel>(ex.Message, ex);
            }
        }
        #endregion

        #region Create

        public async ValueTask<Guid> CreateAsync(MotherboardCreateModel model, string idempotencyKey, CancellationToken cancellationToken)
        {
            const string operation = "MBCreate";

            var requestHash = Convert.ToHexString(
                    SHA256.HashData(
                            JsonSerializer.SerializeToUtf8Bytes(idempotencyKey)
                        )
                );

            var isExist = await context.idempotencyRecords.AsNoTracking().SingleOrDefaultAsync(x => x.Operation == operation && x.Key == idempotencyKey,
                                                                                               cancellationToken);

            if (isExist is not null) return GetPreviousResult(isExist, requestHash);

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                MotherboardDbModel newModel = new();
                newModel.ID = Guid.NewGuid();
                newModel.Name = model.Name;
                newModel.Socket = model.Socket;
                newModel.MaxCpuFrequency = model.MaxCpuFrequency;
                newModel.IntegrationGpu = model.IntegrationGpu;
                newModel.MaxRamFrequency = model.MaxRamFrequency;
                newModel.MaxRamSlot = model.MaxRamSlot;
                newModel.IsLive = model.IsLive;
                newModel.VideoSlot = model.VideoSlot;
                var payload = new PayloadKafka("MBCreated",
                                               newModel.ID,
                                               newModel.Name,
                                               DateTimeOffset.UtcNow).Convert2Json();

                var kafkaMessage = new OutboxMessage
                {
                    EventType = "MBCreated",
                    Topic = "mb-topic",
                    AggregateId = newModel.ID.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    Payload = payload
                };

                var idempotencyKey2Db = new IdempotencyRecord {
                    CreateAt = DateTime.UtcNow,
                    ID = Guid.NewGuid(),
                    Key = idempotencyKey,
                    Operation = operation,
                    RequestHash = requestHash,
                    ResourceId = newModel.ID
                };


                await context.idempotencyRecords.AddAsync(idempotencyKey2Db);
                await context.Motherboards.AddAsync(newModel);
                await context.OutboxMessages.AddAsync(kafkaMessage);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return newModel.ID;
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException {
                SqlState: PostgresErrorCodes.UniqueViolation
            }) {
                await transaction.RollbackAsync();
                context.ChangeTracker.Clear();

                var concurrentResult = await context.idempotencyRecords.AsNoTracking().SingleOrDefaultAsync(x => x.Operation == operation && x.Key == idempotencyKey,
                                                                                                            cancellationToken);

                if (concurrentResult is null) throw;

                return GetPreviousResult(concurrentResult,requestHash);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new CreateOperationException<MotherboardCreateModel>(ex.Message, ex);
            }
        }
        #endregion

        #region Remove

        public async ValueTask<bool> RemoveAsync(Guid id)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                MotherboardDbModel? model = await context.Motherboards.FirstOrDefaultAsync(x => x.ID == x.ID);
                if (model is null) return false;

                var payload = new PayloadKafka("MBRemoved",
                               model.ID,
                               model.Name,
                               DateTimeOffset.UtcNow).Convert2Json();

                var kafkaMessage = new OutboxMessage
                {
                    EventType = "MBRemoved",
                    Topic = "mb-topic",
                    AggregateId = model.ID.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    Payload = payload
                };

                context.Motherboards.Remove(model);
                await context.OutboxMessages.AddAsync(kafkaMessage);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new RemoveOperationException<MotherboardDbModel>("Item not found, remove aborted", ex);
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
