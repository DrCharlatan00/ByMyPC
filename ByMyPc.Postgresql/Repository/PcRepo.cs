using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Exceptions;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Text.Json;


namespace ByMyPc.Postgresql.Repository
{
    public class PcRepo(PgContext context) : IPcRepo
    {
        private readonly PgContext context = context;

        #region Get

        public async IAsyncEnumerable<PcDbModel> GetFullAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in context.PCs.AsNoTracking()
                .Include(x => x.Cpu)
                .Include(x => x.Motherboard)
                .Include(x => x.Gpu)
                .Include(x => x.PSU)
                .Include(x => x.Rams)
                .Include(x => x.HDDs)
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async IAsyncEnumerable<PcSmallModel> GetSmallModelsAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in context.PCs
                .AsNoTracking()
                .Select(x => new PcSmallModel(
                    x.ID,
                    x.Name,
                    x.CpuId,
                    x.GpuId,
                    x.MotherboardId,
                    x.PSUId,
                    x.Rams.Select(r => r.RamId).ToList(),
                    x.HDDs.Select(h => h.HddId).ToList()
                ))
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async Task<PcDbModel?> GetByID(Guid id)
        {
            var item = await context.PCs.AsNoTracking()
                .Include(x => x.Cpu)
                .Include(x => x.Motherboard)
                .Include(x => x.Gpu)
                .Include(x => x.PSU)
                .Include(x => x.Rams)
                .Include(x => x.HDDs)
                .FirstOrDefaultAsync(x => x.ID == id);
            return item;
        }

        public async Task<IEnumerable<PcDbModel>> GetPcFullWithPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.PCs.AsNoTracking()
                .Include(x => x.Cpu)
                .Include(x => x.Motherboard)
                .Include(x => x.Gpu)
                .Include(x => x.PSU)
                .Include(x => x.Rams)
                .Include(x => x.HDDs)
                .OrderBy(x => x.ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<PcSmallModel>> GetPcSmallWithPagination(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.PCs.AsNoTracking()
                .OrderBy(x => x.ID)
                   .Select(x => new PcSmallModel(
                    x.ID,
                    x.Name,
                    x.CpuId,
                    x.GpuId,
                    x.MotherboardId,
                    x.PSUId,
                    x.Rams.Select(r => r.RamId).ToList(),
                    x.HDDs.Select(h => h.HddId).ToList()
                ))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<PcDbModel>?> SearchByName(string name, CancellationToken cancellationToken)
        {
            return await context.PCs.AsNoTracking()
                .Where(x => x.Name.Contains(name))
                .Include(x => x.Cpu)
                .Include(x => x.Motherboard)
                .Include(x => x.Gpu)
                .Include(x => x.PSU)
                .Include(x => x.Rams)
                .Include(x => x.HDDs)
                .OrderBy(x => x.ID)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<PcSmallModel>?> SearchByNameWithPag(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.PCs.AsNoTracking()
                .Where(x => x.Name.Contains(name))
                .OrderBy(x => x.ID)
                 .Select(x => new PcSmallModel(
                    x.ID,
                    x.Name,
                    x.CpuId,
                    x.GpuId,
                    x.MotherboardId,
                    x.PSUId,
                    x.Rams.Select(r => r.RamId).ToList(),
                    x.HDDs.Select(h => h.HddId).ToList()
                ))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<PcDbModel>> GetByFilter(PcFilterModel filter, CancellationToken cancellationToken)
        {
            IQueryable<PcDbModel> models = context.PCs.AsNoTracking().Include(x => x.Cpu)
                .Include(x => x.Motherboard)
                .Include(x => x.Gpu)
                .Include(x => x.PSU)
                .Include(x => x.Rams)
                .Include(x => x.HDDs);

            if (filter.namePc is not null) models = models.Where(x => x.Name.Contains(filter.namePc));

            if (filter.countCore is not null) models = models.Where(x => x.Cpu.Count_Cores == filter.countCore);

            if (filter.socket is not null) models = models.Where(x => x.Motherboard.Socket.Contains(filter.socket));

            if (filter.videoMemorySize is not null) models = models.Where(x => x.Gpu.VideoMemorySize == filter.videoMemorySize);

            if (filter.powerWatt is not null) models = models.Where(x => x.PSU.PowerWatt == filter.powerWatt);

            return await models.OrderBy(x => x.ID).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<PcSmallModel>> GetByFilterWithPagination(PcFilterModel filter, int page, int pageSize, CancellationToken cancellationToken)
        {
            IQueryable<PcDbModel> models = context.PCs.AsNoTracking().Include(x => x.Cpu)
                .Include(x => x.Motherboard)
                .Include(x => x.Gpu)
                .Include(x => x.PSU)
                .Include(x => x.Rams)
                .Include(x => x.HDDs);

            if (filter.namePc is not null) models = models.Where(x => x.Name.Contains(filter.namePc));

            if (filter.countCore is not null) models = models.Where(x => x.Cpu.Count_Cores == filter.countCore);

            if (filter.socket is not null) models = models.Where(x => x.Motherboard.Socket.Contains(filter.socket));

            if (filter.videoMemorySize is not null) models = models.Where(x => x.Gpu.VideoMemorySize == filter.videoMemorySize);

            if (filter.powerWatt is not null) models = models.Where(x => x.PSU.PowerWatt == filter.powerWatt);

            return await models.OrderBy(x => x.ID)
                 .Select(x => new PcSmallModel(
                    x.ID,
                    x.Name,
                    x.CpuId,
                    x.GpuId,
                    x.MotherboardId,
                    x.PSUId,
                    x.Rams.Select(r => r.RamId).ToList(),
                    x.HDDs.Select(h => h.HddId).ToList()
                ))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        #endregion

        #region Update
        public async Task<PcDbModel?> UpdateAsync(PcUpdateModel model)
        {
            var old = await context.PCs.FirstOrDefaultAsync(x => x.ID == model.ID);
            if (old is null) return null;

            old.Name = model.Name ?? old.Name;

            var payload = new PayloadKafka(
                 "PCUpdated",
                 old.ID,
                 old.Name,
                 DateTimeOffset.UtcNow
            ).Convert2Json();

            var outboxMess = new OutboxMessage
            {
                EventType = "PCUpdated",
                Topic = "pc-topic",
                CreatedAt = DateTime.UtcNow,
                AggregateId = old.ID.ToString(),
                Payload = payload,
                
            };

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await context.OutboxMessages.AddAsync(outboxMess);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new UpdateOperationException<PcDbModel>("Can't save new Update", ex);
            }
            return old;
        }

        #endregion

        #region Create
        public async ValueTask<Guid> CreateAsync(PcCreateModel model)
        {
            PcDbModel newModel = new PcDbModel
            {
                ID = Guid.NewGuid(),
                Name = model.Name is null ? "N?A" : model.Name,
            };

            var payload = new PayloadKafka(
                "PCCreated",
                newModel.ID,
                newModel.Name,
                DateTimeOffset.UtcNow
                ).Convert2Json();

            var outboxMessage = new OutboxMessage
            {
                EventType = "PCCreated",
                Topic = "pc-topic",
                AggregateId = newModel.ID.ToString(),
                Payload = payload,
                CreatedAt = DateTime.UtcNow
            };
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await context.PCs.AddAsync(newModel);
                await context.OutboxMessages.AddAsync(outboxMessage);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new CreateOperationException<PcDbModel>("Creating is fail", ex);
            }
            return newModel.ID;
        }
        #endregion

        #region Remove
        public async ValueTask<bool> RemoveAsync(Guid id)
        {
            var old = await context.PCs.FirstOrDefaultAsync(x => x.ID == id);
            if (old is null) return false;

            var payload = new PayloadKafka(
                "PCRemoved",
                id,
                old.Name,
                DateTimeOffset.UtcNow
            ).Convert2Json();

            var outboxMess = new OutboxMessage {
                EventType = "PCRemoved",
                Topic = "pc-topic",
                Payload = payload,
                CreatedAt = DateTime.UtcNow,
                AggregateId = id.ToString()
            };

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                context.PCs.Remove(old);
                await context.OutboxMessages.AddAsync(outboxMess);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new RemoveOperationException<PcDbModel>("Item remove is fail", ex);
            }
            return true;
        }
        #endregion

        #region Attach Methods
        public async ValueTask<bool> AttachCPUAsync(Guid idPc, Guid IdCPU)
        {
            var CPU = await context.CPUs.AsNoTracking().FirstOrDefaultAsync(x => x.ID == IdCPU);
            if (CPU is null) return false;

            var PC = await context.PCs.Include(x => x.Cpu).FirstOrDefaultAsync(x => x.ID == idPc);
            if (PC is null) return false;

            if (PC.Cpu is not null) throw new OperationsException<PcDbModel>("In this PC, CPU Already Attached", true);

            bool cpuIsAttached = await context.PCs
                .AnyAsync(x => x.CpuId == IdCPU);

            if (cpuIsAttached)
                throw new OperationsException<CpuDbModel>(
                    "CPU is already attached to another PC", true);


            try
            {
                PC.CpuId = IdCPU;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new OperationsException<PcDbModel>("Cpu not attached", false, ex);
            }
            return true;

        }

        public async ValueTask<bool> AttachGPUAsync(Guid idPc, Guid IdGPU)
        {
            var CPU = await context.GPUs.AsNoTracking().FirstOrDefaultAsync(x => x.ID == IdGPU);
            if (CPU is null) return false;

            var PC = await context.PCs.Include(x => x.Cpu).FirstOrDefaultAsync(x => x.ID == idPc);
            if (PC is null) return false;

            if (PC.Cpu is not null) throw new OperationsException<PcDbModel>("In this PC, GPU Already Attached", true);

            bool cpuIsAttached = await context.PCs
                .AnyAsync(x => x.GpuId == IdGPU);

            if (cpuIsAttached)
                throw new OperationsException<GpuDbModel>(
                    "GPU is already attached to another PC", true);


            try
            {
                PC.GpuId = IdGPU;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new OperationsException<PcDbModel>("Gpu not attached", false, ex);
            }
            return true;

        }

        public async ValueTask<bool> AttachMotherboardAsync(Guid idPc, Guid IdMotherboard)
        {
            var CPU = await context.Motherboards.AsNoTracking().FirstOrDefaultAsync(x => x.ID == IdMotherboard);
            if (CPU is null) return false;

            var PC = await context.PCs.Include(x => x.Cpu).FirstOrDefaultAsync(x => x.ID == idPc);
            if (PC is null) return false;

            if (PC.Cpu is not null) throw new OperationsException<PcDbModel>("In this PC, Motherboard Already Attached", true);

            bool cpuIsAttached = await context.PCs
                .AnyAsync(x => x.MotherboardId == IdMotherboard);

            if (cpuIsAttached)
                throw new OperationsException<GpuDbModel>(
                    "Motherboard is already attached to another PC", true);


            try
            {
                PC.MotherboardId = IdMotherboard;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new OperationsException<PcDbModel>("Motherboard not attached", false, ex);
            }
            return true;
        }

        public async ValueTask<bool> AttachPSUAsync(Guid idPc, Guid IdPSU)
        {
            var CPU = await context.Psu.AsNoTracking().FirstOrDefaultAsync(x => x.ID == IdPSU);
            if (CPU is null) return false;

            var PC = await context.PCs.Include(x => x.Cpu).FirstOrDefaultAsync(x => x.ID == idPc);
            if (PC is null) return false;

            if (PC.Cpu is not null) throw new OperationsException<PcDbModel>("In this PC, PSU Already Attached", true);

            bool cpuIsAttached = await context.PCs
                .AnyAsync(x => x.PSUId == IdPSU);

            if (cpuIsAttached)
                throw new OperationsException<GpuDbModel>(
                    "PSU is already attached to another PC", true);


            try
            {
                PC.PSUId = IdPSU;
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new OperationsException<PcDbModel>("PSU not attached", false, ex);
            }
            return true;

        }
        #endregion

        #region Detach Methods

        public async ValueTask<bool> DeattachCPUAsync(Guid idPc, Guid idCPU)
        {
            var cpuExists = await context.CPUs
                .AnyAsync(x => x.ID == idCPU);

            if (!cpuExists)
                return false;

            var pc = await context.PCs
                .FirstOrDefaultAsync(x => x.ID == idPc);

            if (pc is null)
                return false;

            if (pc.CpuId != idCPU)
                throw new OperationsException<CpuDbModel>(
                    "CPU is not attached to this PC", true);

            try
            {
                pc.CpuId = null;

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new OperationsException<PcDbModel>(
                    "CPU could not be detached", false, ex);
            }

            return true;
        }

        public async ValueTask<bool> DeattachGPUAsync(Guid idPc, Guid idGPU)
        {
            var cpuExists = await context.GPUs
                .AnyAsync(x => x.ID == idGPU);

            if (!cpuExists)
                return false;

            var pc = await context.PCs
                .FirstOrDefaultAsync(x => x.ID == idPc);

            if (pc is null)
                return false;

            if (pc.GpuId != idGPU)
                throw new OperationsException<GpuDbModel>(
                    "GPU is not attached to this PC", true);

            try
            {
                pc.GpuId = null;

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new OperationsException<PcDbModel>(
                    "GPU could not be detached", false, ex);
            }

            return true;
        }

        public async ValueTask<bool> DeattachMotherbordAsync(Guid idPc, Guid idMotherboard)
        {
            var cpuExists = await context.Motherboards
                .AnyAsync(x => x.ID == idMotherboard);

            if (!cpuExists)
                return false;

            var pc = await context.PCs
                .FirstOrDefaultAsync(x => x.ID == idPc);

            if (pc is null)
                return false;

            if (pc.MotherboardId != idMotherboard)
                throw new OperationsException<MotherboardDbModel>(
                    "Motherboard is not attached to this PC", true);

            try
            {
                pc.MotherboardId = null;

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new OperationsException<PcDbModel>(
                    "Motherboard could not be detached", false, ex);
            }

            return true;
        }

        public async ValueTask<bool> DeattachPSUAsync(Guid idPc, Guid idPSU)
        {
            var cpuExists = await context.Psu
                .AnyAsync(x => x.ID == idPSU);

            if (!cpuExists)
                return false;

            var pc = await context.PCs
                .FirstOrDefaultAsync(x => x.ID == idPc);

            if (pc is null)
                return false;

            if (pc.PSUId != idPSU)
                throw new OperationsException<PSUDbModel>(
                    "PSU is not attached to this PC", true);

            try
            {
                pc.PSUId = null;

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new OperationsException<PcDbModel>(
                    "PSU could not be detached", false, ex);
            }

            return true;
        }

        #endregion

    }
}
