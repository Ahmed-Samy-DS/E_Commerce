using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Data;
using E_Commerce.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Repositories
{
    internal class GenericRepository<TEntity, Tkey>(StoreDbContext dbContext) : IGenericRepository<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {

        public void Add(TEntity entity) => dbContext.Set<TEntity>().Add(entity);

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
                => await dbContext.Set<TEntity>().ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecifications<TEntity, Tkey> spec, CancellationToken cancellationToken = default)
        {

            var query = SpecificationEvaluator.CreateQuery(dbContext.Set<TEntity>(), spec);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync(Tkey id, CancellationToken cancellationToken = default)
                => await dbContext.Set<TEntity>().FindAsync(id, cancellationToken);

        public async Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, Tkey> spec, CancellationToken cancellationToken = default)
        {

            var query = SpecificationEvaluator.CreateQuery(dbContext.Set<TEntity>(), spec);
            return await query.FirstOrDefaultAsync();

        }

        public void Remove(TEntity entity) => dbContext.Set<TEntity>().Remove(entity);

        public void Update(TEntity entity)=> dbContext.Set<TEntity>().Update(entity);
    }
}
