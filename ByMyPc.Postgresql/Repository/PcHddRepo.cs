using ByMyPc.Postgresql.Exceptions;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByMyPc.Postgresql.Repository
{
    public class PcHddRepo(PgContext context) : IPcHddRepo
    {
        private readonly PgContext context = context;

        public async ValueTask<bool> AttachHddToPc(Guid PcId, Guid HDDId)
        {

            var HDD = await context.HDDs.AsNoTracking().FirstOrDefaultAsync(x => x.ID == HDDId);
            if (HDD is null) return false;

            var PC = await context.PCs.AsNoTracking().FirstOrDefaultAsync(x => x.ID == PcId);
            if (PC is null) return false;

            bool IsAttached = await context.PcHdds.AsNoTracking().FirstOrDefaultAsync(x => x.HddId == HDDId) is not null ? true : false;

            if (IsAttached) {
                throw new OperationsException<HDDDbModel>("Disk is Attached",true);
            }

            PcHddDbModel newPcHdd = new PcHddDbModel(PcId, HDDId);

            var payloadAttach = new PayloadKafka("HDDAttached",
                                                       HDD.ID,
                                                       HDD.Name,
                                                       DateTimeOffset.UtcNow).Convert2Json();

            var kafkaMessageAttach = new OutboxMessage
            {
                EventType = "HDDAttached",
                Topic = "hdd-topic",
                CreatedAt = DateTime.UtcNow,
                AggregateId = HDD.ID.ToString(),
                Payload = payloadAttach
            };
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await context.PcHdds.AddAsync(newPcHdd);
                await context.OutboxMessages.AddAsync(kafkaMessageAttach);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new OperationsException<PcHddDbModel>("In Attach HDD to PC operation exception",false,ex);
            }
            return true;
        }

        public async ValueTask<bool> DeAtthachHDD(Guid PcId, Guid HDDId)
        {
            var HDD = await context.HDDs.AsNoTracking().FirstOrDefaultAsync(x => x.ID == HDDId);
            if (HDD is null) return false;

            var PC = await context.PCs.AsNoTracking().FirstOrDefaultAsync(x => x.ID == PcId);
            if (PC is null) return false;

            var PcHDD = await context.PcHdds.FirstOrDefaultAsync(x => (x.PcId == PcId || x.HddId == HDDId));
            if (PcHDD is null) return false;

            var payloadAttach = new PayloadKafka("HDDDeattached",
                                           HDD.ID,
                                           HDD.Name,
                                           DateTimeOffset.UtcNow).Convert2Json();

            var kafkaMessageAttach = new OutboxMessage
            {
                EventType = "HDDDeattached",
                Topic = "hdd-topic",
                CreatedAt = DateTime.UtcNow,
                AggregateId = HDD.ID.ToString(),
                Payload = payloadAttach
            };
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                context.PcHdds.Remove(PcHDD);
                await context.OutboxMessages.AddAsync(kafkaMessageAttach);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new OperationsException<PcHddDbModel>("In DeAttach HDD to PC operation exception", false, ex);
            }
            return true;
        }

    }
}
