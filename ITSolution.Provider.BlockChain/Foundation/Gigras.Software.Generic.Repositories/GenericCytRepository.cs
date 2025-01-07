using Gigras.Software.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gigras.Software.Generic.Repositories
{
    public class GenericCytRepository<T> : IGenericCytRepository<T> where T : class
    {
        protected readonly CytContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericCytRepository(CytContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return _context.Set<T>().Where(filter).ToListAsync();
        }

        public Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
                                                      Expression<Func<T, object>> orderBy = null, int numrec = -1, int startrec = -1)
        {
            var query = _context.Set<T>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }
            if (startrec != -1)
                query = query.Skip(startrec);
            if (numrec != -1)
                query = query.Take(numrec);
            return query.ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(
                            Expression<Func<T, bool>> filter = null, // Filtering condition
                            params Expression<Func<T, object>>[] includes // Navigation properties to include
                        )
        {
            IQueryable<T> query = _dbSet;

            // Apply filter
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Apply includes for navigation properties
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
                                                                        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, // Sorting condition
                                                                        params Expression<Func<T, object>>[] includes
                                                            )
        {
            IQueryable<T> query = _dbSet;

            // Apply filter
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Apply includes
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // Apply ordering
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _context.Set<T>();

            // Apply the include function if provided
            if (include != null)
            {
                query = include(query);
            }

            // Retrieve the entity by ID
            return await query.AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object?>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            // Apply the includes
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // Retrieve the entity by ID
            return await query.AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<T?> GetByIdAsync(
                int id,
                Expression<Func<T, bool>>? filter = null,
                params Expression<Func<T, object?>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            // Apply the includes
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // Apply the filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (id == 0)
            {
                return await query.AsNoTracking()
                                  .FirstOrDefaultAsync();
            }
            else
            {
                // Retrieve the entity by ID
                return await query.AsNoTracking()
                                  .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
            }
        }


        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, bool IsSave = true)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                if (IsSave)
                    await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteManyAsync(List<int> ids, bool IsSave = true)
        {
            // Use reflection to find the ID property of the entity type
            var idProperty = typeof(T).GetProperties()
                                      .FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)
                                                        || p.Name.Equals(typeof(T).Name + "Id", StringComparison.OrdinalIgnoreCase));

            if (idProperty == null)
            {
                throw new InvalidOperationException("No ID property found for the entity type.");
            }

            // Find the entities with matching IDs
            var entities = await _dbSet.Where(e => ids.Contains((int)idProperty.GetValue(e))).ToListAsync();

            if (!entities.Any())
            {
                Console.WriteLine("No entities found with the provided IDs.");
                return; // No entities to delete
            }

            _dbSet.RemoveRange(entities);
            if (IsSave)
                await _context.SaveChangesAsync();
        }

        public async Task DeleteByConditionAsync(Func<T, bool> condition, bool IsSave = true)
        {
            var entities = _dbSet.Where(condition).ToList();

            if (entities.Any())
            {
                _dbSet.RemoveRange(entities);
                if (IsSave)
                    await _context.SaveChangesAsync();
            }
        }
    }
}