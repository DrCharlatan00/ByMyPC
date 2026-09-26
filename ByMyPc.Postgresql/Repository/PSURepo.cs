using ByMyPc.Postgresql.CRUDModel.Operation;
using ByMyPc.Postgresql.CRUDModel.SmallModels;
using ByMyPc.Postgresql.Exceptions;
using ByMyPc.Postgresql.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByMyPc.Postgresql.Repository
{
    public class PSURepo(PgContext context)
    {
        private readonly PgContext context = context;

        #region Get

        public async IAsyncEnumerable<PSUDbModel> GetFullAsyncEnumerable([EnumeratorCancellation] CancellationToken cancellation)
        {
            await foreach (var item in context.Psu.AsNoTracking()
                                                  .AsAsyncEnumerable()
                                                  .WithCancellation(cancellation))
            {
                yield return item;
            }
        }
        public async IAsyncEnumerable<PSUSmallModel> GetSmallAsyncEnumerable([EnumeratorCancellation] CancellationToken cancellation)
        {
            await foreach (var item in context.Psu.AsNoTracking()
                                                  .Select(x => new PSUSmallModel(x.ID, x.Name, x.PowerWatt, x.IsLive))
                                                  .AsAsyncEnumerable()
                                                  .WithCancellation(cancellation))
            {
                yield return item;
            }
        }


        public async Task<PSUDbModel?> GetById(Guid id) {
            return await context.Psu.AsNoTracking().FirstOrDefaultAsync(x => x.ID == id);
        }

        public async IAsyncEnumerable<PSUSmallModel> SearchByNameSmallAsyncEnumerable(string name,[EnumeratorCancellation] CancellationToken cancellation) {
            await foreach (var item in context.Psu.AsNoTracking()
                                                  .Where(x => x.Name == name)
                                                  .OrderBy(x => x.ID)
                                                  .Select(x => new PSUSmallModel(x.ID, x.Name, x.PowerWatt, x.IsLive))
                                                  .AsAsyncEnumerable()
                                                  .WithCancellation(cancellation))
            {
                yield return item;
            }

        }

        public async Task<IEnumerable<PSUSmallModel>> GetSmallModelsAsyncWithPag(int page, int pageSize, CancellationToken cancellation) {
            return await context.Psu.AsNoTracking()
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .OrderBy(x => x.ID)
                                    .Select(x => new PSUSmallModel(x.ID, x.Name, x.PowerWatt, x.IsLive))
                                    .ToListAsync(cancellation);
        }

        #endregion

        #region Update
        public async Task<PSUDbModel?> UpdateAsync(PSUUpdateModel model) {
            
                var old = await context.Psu.FirstOrDefaultAsync(x => x.ID == model.id);
                if (old is null) return null;
            try
            {
                old.Name = model.Name ?? old.Name;
                old.PowerWatt = model.PowerWatt ?? old.PowerWatt;
                old.Size = model.Size ?? old.Size;
                old.IsLive = model.IsLive;
                old.IsModular = model.IsModular ?? old.IsModular;
                old.IsСertified = model.IsCertified ?? old.IsСertified;
            }
            catch (Exception ex) {
                throw new UpdateOperationException<PSUUpdateModel>("Can't change params",ex);
            }
            try
            {
                await context.SaveChangesAsync();
                return old;
            }
            catch (Exception ex) {
                throw new UpdateOperationException<PSUDbModel>("Update is failed", ex);
            }

        }
        #endregion
        #region Create
        public async Task<Guid> CreateAsync(PSUCreateModel model) {
            PSUDbModel NewPsu = new PSUDbModel {
                ID = Guid.NewGuid(),
                Name = model.Name,
                PowerWatt = model.PowerWatt,
                IsLive = model.IsLive,
                Size = model.Size,
                IsModular = model.IsModular,
                IsСertified = model.IsCertified,
            };

            try
            {
                await context.Psu.AddAsync(NewPsu);
                await context.SaveChangesAsync();
                return NewPsu.ID;
            }
            catch (Exception ex) {
                throw new CreateOperationException<PSUDbModel>("Can't Create Model in db",ex);
            }
        }
        #endregion

        #region Remove
        public async ValueTask<bool> RemoveAsync(Guid id) {
            var rm = await context.Psu.FirstOrDefaultAsync(x => x.ID == id);
            if (rm is null) return false;
            try
            {
                context.Psu.Remove(rm);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) {
                throw new RemoveOperationException<PSUDbModel>("Item not removed", ex);
            }
        }
        #endregion
    }
}
