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
            try
            {
                await context.SaveChangesAsync();
                return old;
            }
            catch (Exception ex)
            {
                throw new UpdateOperationException<HDDDbModel>(ex.Message, ex);
            }
        }
        #endregion

        #region Create
        public async Task<Guid> CreateAsync(HDDCreateModel createModel)
        {
            HDDDbModel newModel = new HDDDbModel
            {
                ID = Guid.NewGuid(),
                Name = createModel.Name,
                GbSize = createModel.GbSize,
                connector = createModel.connector
            }; try
            {
                await context.HDDs.AddAsync(newModel);
                await context.SaveChangesAsync();
                return newModel.ID;
            }
            catch (Exception ex)
            {
                throw new CreateOperationException<HDDDbModel>(ex.Message, ex);
            }
        }
        #endregion

        #region Remove
        public async Task RemoveAsync(Guid id)
        {
            var item = await context.HDDs.FirstOrDefaultAsync(x => x.ID == id);
            if (item is null) throw new RemoveOperationException<HDDDbModel>("Item not found, remove aborted");
            try
            {
                context.HDDs.Remove(item);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
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
                try
                {
                    await context.HDDs.AddAsync(newModel);
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
                    await context.PcHdds.AddAsync(new PcHddDbModel(PcId, newModel.ID));
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
