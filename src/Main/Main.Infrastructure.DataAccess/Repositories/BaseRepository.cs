
using Main.Domain.entities.common;
using Main.Domain.InterfacesRepository;
using Main.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;

namespace Main.Infrastructure.DataAccess.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {

        internal readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public BaseRepository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public async Task<T> GetFirstAsync(Expression<Func<T, bool>> filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = dbSet;

            if (includeProperties != null)
            {
                query = query.Include(includeProperties);
            }

            if (filter != null)
            {
                return await query.FirstOrDefaultAsync(filter, cancellationToken);
            }
            else
            {
                return await query.FirstOrDefaultAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, string? includeProperties = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = dbSet;

            if (includeProperties != null)
            {
                query = query.Include(includeProperties);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken)
        {
            try
            {
                await dbSet.AddAsync(entity, cancellationToken);

                await _db.SaveChangesAsync(cancellationToken);

                return entity;
            }catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            dbSet.Update(entity);

            await _db.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken)
        {
            var entity = dbSet.Where(filter);

            dbSet.RemoveRange(entity);
           
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> IsExistsAsync(Expression<Func<T, bool>> filter = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = dbSet;

            return await query.AnyAsync(filter, cancellationToken);
        }
    }
}
