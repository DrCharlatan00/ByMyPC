using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Exceptions;
using ByMyPc.Postgresql.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByMyPc.Postgresql.Repository
{
    public class PcRepo(PgContext context)
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

        public async Task<IEnumerable<PcDbModel>> GetPcFullWithPagination(int page, int pageSize,CancellationToken cancellationToken)
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

        public async Task<IEnumerable<PcSmallModel>> GetByFilterWithPagination(PcFilterModel filter,int page,int pageSize,CancellationToken cancellationToken)
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

            return await  models.OrderBy(x => x.ID)
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

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new UpdateOperationException<PcDbModel>("Can't save new Update",ex);
            }
            return old;
        }

        #endregion

        #region Create
        public async ValueTask<Guid> CreateAsync(PcCreateModel model)
        {
            PcDbModel newModel = new PcDbModel {
                ID = Guid.NewGuid(),
                Name = model.Name is null ? "N?A" : model.Name,
            };
            try
            {
                await context.PCs.AddAsync(newModel);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new CreateOperationException<PcDbModel>("Creating is fail",ex);
            }
            return newModel.ID;
        }
        #endregion

        #region Remove
        public async ValueTask<bool> RemoveAsync(Guid id)
        {
            var old = await context.PCs.FirstOrDefaultAsync(x => x.ID == id);
            if (old is null) return false;
            try
            {
                context.PCs.Remove(old);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RemoveOperationException<PcDbModel>("Item remove is fail", ex);
            }
            return true;
        }
        #endregion


    }
}
