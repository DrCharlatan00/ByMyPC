using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Exceptions;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

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

            old.Name = model.Name ?? old.Name;
            old.VideoMemorySize = model.VideoMemorySize ?? old.VideoMemorySize;
            old.VideoSlot = model.VideoSlot ?? old.VideoSlot;
            old.MemoryBus = model.MemoryBus ?? old.MemoryBus;
            old.TypeConnector = model.TypeConnector ?? old.TypeConnector;
            old.TypeMemory = model.TypeMemory ?? old.TypeMemory;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new UpdateOperationException<GpuDbModel>("Can't Save new model", ex);
            }
            return old;

        }

        #endregion

        #region Create
        public async ValueTask<Guid> CreateAsync(GPUCreateModel model)
        {
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

            try
            {
                await context.GPUs.AddAsync(gpuDbModel);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new CreateOperationException<GpuDbModel>("Can;t save new model", ex);
            }
            return gpuDbModel.ID;
        }
        #endregion


        #region Remove 
        public async ValueTask<bool> RemoveAsync(Guid id)
        {
            var item = await context.GPUs.FirstOrDefaultAsync(x => x.ID == id);
            if (item is null) return false;

            try
            {
                context.GPUs.Remove(item);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new RemoveOperationException<GpuDbModel>("Item not removed", ex);
            }
        }
        #endregion
    }
}
