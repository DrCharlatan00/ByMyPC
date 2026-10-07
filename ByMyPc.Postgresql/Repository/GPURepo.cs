using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByMyPc.Postgresql.Repository
{
    public class GPURepo(PgContext context)
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
                .Select(x => new GPUSmallModel(x.ID, x.Name, x.VideoMemorySize))
                .OrderBy(x => x.ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<GpuDbModel>?> SearchByName(string name, CancellationToken cancellationToken) 
        {
            return await context.GPUs.AsNoTracking().Where(x => x.Name.Contains(name)).OrderBy(x => x.ID).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<GPUSmallModel>?> SearchByNameWithPag(string name, int page, int pageSize,CancellationToken cancellationToken)
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


        public async Task<IEnumerable<GPUSmallModel>> GetSmallByFilter(GPUFilterModel filterModel, int page, int pageSize,CancellationToken cancellationToken)
        {
            IQueryable<GpuDbModel> query = context.GPUs.AsNoTracking();

            if (filterModel.Name is not null) query = query.Where(x => x.Name.Contains(filterModel.Name));

            if (filterModel.TypeConnector is not null) query = query.Where(x => x.TypeConnector == filterModel.TypeConnector);

            if (filterModel.VideoMemorySize is not null) query = query.Where(x => x.VideoMemorySize == filterModel.VideoMemorySize);

            if (filterModel.VideoSlot is not null) query = query.Where(x => x.VideoSlot == filterModel.VideoSlot);

            if (filterModel.MemoryBus is not null) query = query.Where(x => x.MemoryBus == filterModel.MemoryBus);

            if (filterModel.TypeMemory is not null) query = query.Where(x => x.TypeMemory == filterModel.TypeMemory);

            return await query.Select(x => new GPUSmallModel(x.ID, x.Name, x.VideoMemorySize))
                .OrderBy(x => x.ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        #endregion


        
    }
}
