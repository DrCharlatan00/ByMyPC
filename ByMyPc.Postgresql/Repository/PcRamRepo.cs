using ByMyPc.Postgresql.Exceptions;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using Microsoft.EntityFrameworkCore;


namespace ByMyPc.Postgresql.Repository
{
    public class PcRamRepo(PgContext context) : IPcRamRepo
    {
        private readonly PgContext context = context;

        #region Attach
        public async ValueTask<bool> AttachAsync(Guid PcId, Guid RamID)
        {
            var pc = await context.PCs.AsNoTracking().FirstOrDefaultAsync(x => x.ID == PcId);
            if (pc is null) return false;

            var ram = await context.RAMs.AsNoTracking().FirstOrDefaultAsync(x => x.ID == RamID);
            if (ram is null) return false;

            var isAttached = await context.PcRams.AsNoTracking().FirstOrDefaultAsync(x => x.RamId == RamID);
            if (isAttached is not null) throw new OperationsException<RamDbModel>("RAM Is Attached", true);

            PcRamDbModel model = new PcRamDbModel
            {
                PcId = PcId,
                RamId = RamID,
            };
            var payload = new PayloadKafka("RAMAttached",
                               componentID: ram.ID,
                               name: ram.Name,
                               occurredAt: DateTimeOffset.UtcNow).Convert2Json();

            var kafkaMessage = new OutboxMessage
            {
                EventType = "RAMAttached",
                Topic = "ram-topic",
                AggregateId = ram.ID.ToString(),
                CreatedAt = DateTime.UtcNow,
                Payload = payload
            };
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await context.PcRams.AddAsync(model);
                await context.OutboxMessages.AddAsync(kafkaMessage);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new OperationsException<PcRamDbModel>("Attach is failed", false, ex);
            }
            return true;
        }
        #endregion

        #region DeAttach
        public async Task<bool> DeAttachAsync(Guid PcId, Guid RamID)
        {

            var pc = await context.PCs.AsNoTracking().FirstOrDefaultAsync(x => x.ID == PcId);
            if (pc is null) return false;

            var ram = await context.RAMs.AsNoTracking().FirstOrDefaultAsync(x => x.ID == RamID);
            if (ram is null) return false;
            var isAttached = await context.PcRams.FirstOrDefaultAsync(x => (x.PcId == PcId || x.RamId == RamID));
            if (isAttached is null) throw new OperationsException<RamDbModel>("Ram not Attached", true);

            var payload = new PayloadKafka("RAMDettached",
                   componentID: ram.ID,
                   name: ram.Name,
                   occurredAt: DateTimeOffset.UtcNow).Convert2Json();

            var kafkaMessage = new OutboxMessage
            {
                EventType = "RAMDettached",
                Topic = "ram-topic",
                AggregateId = ram.ID.ToString(),
                CreatedAt = DateTime.UtcNow,
                Payload = payload
            };
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                context.Remove(isAttached);
                await context.OutboxMessages.AddAsync(kafkaMessage);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new OperationsException<RamDbModel>("Ram not DeAttach", false, ex);
            }
            return true;
        }



        #endregion
    }
}
