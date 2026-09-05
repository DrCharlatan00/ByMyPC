using ByMyPc.Postgresql.CRUDModel.FiltersModels;
using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Exceptions;
using ByMyPc.Postgresql.Models;
using ByMyPc.Postgresql.Repository.Intefaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByMyPc.Postgresql.Repository
{
    public class MotherboardRepo(PgContext context) : IMotherboardRepo
    {
        private readonly PgContext context = context;

        public async IAsyncEnumerable<MotherboardSmallDbModel> GetCardMotherboardDbAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in context.Motherboards.AsNoTracking().Select(u => new MotherboardSmallDbModel(u.Name, u.Socket, u.IsLive)).AsAsyncEnumerable().WithCancellation(cancellationToken))
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
                .Select(u => new MotherboardSmallDbModel(u.Name, u.Socket, u.IsLive))
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
                .Select(u => new MotherboardSmallDbModel(u.Name, u.Socket, u.IsLive))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MotherboardSmallDbModel>> SearchByNameMotherboardSmallWithPaginationAsync(string name, int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.Motherboards.AsNoTracking()
                .Where(x => x.Name.Contains(name))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new MotherboardSmallDbModel(u.Name, u.Socket, u.IsLive))
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
                .Select(u => new MotherboardSmallDbModel(u.Name, u.Socket, u.IsLive))
                .ToListAsync(cancellationToken);

        }

        public async Task<MotherboardDbModel?> UpdateAsync(MotherboardUpdateModel model)
        {
            try
            {
                MotherboardDbModel? old = await context.Motherboards.FirstOrDefaultAsync(x => x.ID == model.ID);
                if (old is null) return null;
                old.Name = !string.IsNullOrWhiteSpace(model.Name) ? model.Name : old.Name;
                old.Socket = !string.IsNullOrWhiteSpace(model.Socket) ? model.Socket : old.Socket;
                old.IsLive = model.IsLive;
                await context.SaveChangesAsync();
                return old;
            }
            catch (Exception ex)
            {
                throw new UpdateOperationException<MotherboardDbModel>(ex.Message, ex);
            }
        }

        public async Task<Guid> CreateAsync(MotherboardCreateModel model)
        {
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
                await context.Motherboards.AddAsync(newModel);
                await context.SaveChangesAsync();
                return newModel.ID;
            }
            catch (Exception ex)
            {
                throw new CreateOperationException<MotherboardCreateModel>(ex.Message, ex);
            }
        }

        public async Task RemoveAsync(Guid id)
        {
            try
            {
                MotherboardDbModel? model = await context.Motherboards.FirstOrDefaultAsync(x => x.ID == x.ID);
                if (model is null) throw new RemoveOperationException<MotherboardDbModel>("Item not found, remove aborted");
                context.Motherboards.Remove(model);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RemoveOperationException<MotherboardDbModel>(ex.Message, ex);
            }
        }
    }
}
