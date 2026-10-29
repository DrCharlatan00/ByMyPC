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
    public class HddRepo(PgContext context) : IHddRepo
    {
        private readonly PgContext context = context;

        #region Get
        public async IAsyncEnumerable<HDDSmallModel> GetSmallModelsDbAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (HDDSmallModel? item in context.HDDs.AsNoTracking()
                .Select(x => new HDDSmallModel(x.ID,x.Name, x.GbSize))
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async IAsyncEnumerable<HDDDbModel> GetModelsDbAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (HDDDbModel? item in context.HDDs.AsNoTracking()
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async Task<IEnumerable<HDDDbModel>> GetFullWithPag(int page, int pageSize,CancellationToken cancellationToken)
        {
            return await context.HDDs.AsNoTracking()
                .OrderBy(x => x.ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }


        public async Task<HDDDbModel?> GetByID(Guid id)
        {
            var item = await context.HDDs.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
            return item;
        }

        public async Task<IEnumerable<HDDSmallModel>> GetSmallModelWithPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.HDDs.AsNoTracking()
                .Select(x => new HDDSmallModel(x.ID, x.Name, x.GbSize))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<HDDDbModel>?> SearchByName(string name, CancellationToken cancellationToken)
        {
            return await context.HDDs.AsNoTracking()
                .Where(x => x.Name.Contains(name))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<HDDSmallModel>?> SearchByNameWithPag(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.HDDs.AsNoTracking()
                .Where(x => x.Name.Contains(name))
                .Select(x => new HDDSmallModel(x.ID, x.Name, x.GbSize))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<HDDSmallModel>?> GetSmallByFilter(HDDFilterModel filterModel,int page,int pageSize, CancellationToken cancellationToken)
        {
            IQueryable<HDDDbModel> query = context.HDDs.AsNoTracking();

            if (filterModel.Name is not null) query = query.Where(x => x.Name.Contains(filterModel.Name));

            if (filterModel.GbSize is not null) query = query.Where(x => x.GbSize == filterModel.GbSize);

            if (filterModel.Connector is not null) query = query.Where(x => x.connector == filterModel.Connector);

            return await query.Select(x => new HDDSmallModel(x.ID, x.Name, x.GbSize))
                              .Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<HDDDbModel>?> GetByFilter(HDDFilterModel filterModel, CancellationToken cancellationToken)
        {
            IQueryable<HDDDbModel> query = context.HDDs.AsNoTracking();

            if (filterModel.Name is not null) query = query.Where(x => x.Name.Contains(filterModel.Name));

            if (filterModel.GbSize is not null) query = query.Where(x => x.GbSize == filterModel.GbSize);

            if (filterModel.Connector is not null) query = query.Where(x => x.connector == filterModel.Connector);

            return await query.ToListAsync(cancellationToken);
        }


        #endregion

        #region Update
        public async Task<HDDDbModel?> UpdateAsync(HDDUpdateModel updateModel)
        {
            var old = await context.HDDs.FirstOrDefaultAsync(x => x.ID == updateModel.id);

            if (old is null) return null;

            old.Name = updateModel.Name ?? old.Name;
            old.connector = updateModel.connector ?? old.connector;
            old.GbSize = updateModel.GbSize ?? old.GbSize;

            var payload = new PayloadKafka(
                "HDDUpdated",
                old.ID,
                old.Name,
                DateTimeOffset.UtcNow
                ).Convert2Json();

            var kafkaMessage = new OutboxMessage {
                EventType = "HDDUpdated",
                Topic = "hdd-topic",
                CreatedAt = DateTime.UtcNow,
                AggregateId = old.ID.ToString(),
                Payload = payload
            };
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await context.OutboxMessages.AddAsync(kafkaMessage);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return old;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new UpdateOperationException<HDDDbModel>(ex.Message, ex);
            }
        }
        #endregion

        #region Create
        public async ValueTask<Guid> CreateAsync(HDDCreateModel createModel)
        {
            HDDDbModel newModel = new HDDDbModel
            {
                ID = Guid.NewGuid(),
                Name = createModel.Name,
                GbSize = createModel.GbSize,
                connector = createModel.connector
            };
            var payload = new PayloadKafka(
    "HDDCreated",
    newModel.ID,
    newModel.Name,
    DateTimeOffset.UtcNow
    ).Convert2Json();

            var kafkaMessage = new OutboxMessage
            {
                EventType = "HDDCreated",
                Topic = "hdd-topic",
                CreatedAt = DateTime.UtcNow,
                AggregateId = newModel.ID.ToString(),
                Payload = payload
            };
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await context.HDDs.AddAsync(newModel);
                await context.OutboxMessages.AddAsync(kafkaMessage);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return newModel.ID;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new CreateOperationException<HDDDbModel>(ex.Message, ex);
            }
        }
        #endregion

        #region Remove
        public async ValueTask<bool> RemoveAsync(Guid id)
        {
            var item = await context.HDDs.FirstOrDefaultAsync(x => x.ID == id);
            if (item is null) return false;
            var payload = new PayloadKafka(
"HDDRemoved",
item.ID,
item.Name,
DateTimeOffset.UtcNow
).Convert2Json();

            var kafkaMessage = new OutboxMessage
            {
                EventType = "HDDRemoved",
                Topic = "hdd-topic",
                CreatedAt = DateTime.UtcNow,
                AggregateId = item.ID.ToString(),
                Payload = payload
            };
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                context.HDDs.Remove(item);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new RemoveOperationException<HDDDbModel>(ex.Message, ex);
            }
        }
        #endregion

        #region Other Operation
        public async Task<Guid> CreateAndAttach(HDDCreateModel model, Guid PcId) {
            var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                HDDDbModel newModel = new HDDDbModel
                {
                    ID = Guid.NewGuid(),
                    Name = model.Name,
                    GbSize = model.GbSize,
                    connector = model.connector
                };
                var payload = new PayloadKafka("HDDCreated",
                                               newModel.ID,
                                               newModel.Name,
                                               DateTimeOffset.UtcNow).Convert2Json();

                var kafkaMessage = new OutboxMessage
                {
                    EventType = "HDDCreated",
                    Topic = "hdd-topic",
                    CreatedAt = DateTime.UtcNow,
                    AggregateId = newModel.ID.ToString(),
                    Payload = payload
                };
                try
                {
                    await context.HDDs.AddAsync(newModel);
                    await context.OutboxMessages.AddAsync(kafkaMessage);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex) {
                    await transaction.RollbackAsync();
                    throw new CreateOperationException<HDDCreateModel>("Hdd not create, Create and Attaching is aborted", ex);
                }
                var PC = await context.PCs.AsNoTracking().FirstOrDefaultAsync(x => x.ID == PcId);
                if (PC is null) {
                    await transaction.RollbackAsync();
                    throw new OperationsException<PcDbModel>("Pc is not found, Attaching is abort",true);
                }
                try
                {
                    var payloadAttach = new PayloadKafka("HDDAttached",
                                                         newModel.ID,
                                                         newModel.Name,
                                                         DateTimeOffset.UtcNow).Convert2Json();

                    var kafkaMessageAttach = new OutboxMessage
                    {
                        EventType = "HDDAttached",
                        Topic = "hdd-topic",
                        CreatedAt = DateTime.UtcNow,
                        AggregateId = newModel.ID.ToString(),
                        Payload = payload
                    };

                    await context.PcHdds.AddAsync(new PcHddDbModel(PcId, newModel.ID));
                    await context.OutboxMessages.AddAsync(kafkaMessageAttach);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex) {
                    await transaction.RollbackAsync();
                    throw new OperationsException<PcHddDbModel>("Can't attach hdd to pc, Create and Attaching is aborted",false,ex);
                }
                await transaction.CommitAsync();
                return newModel.ID;
            }
            catch (Exception ex) {
                await transaction.RollbackAsync();
                throw new OperationsException<object>("Error when create or add to pc,Create and Attaching is aborted",false, ex);
            }
        }
        #endregion
    }
}
