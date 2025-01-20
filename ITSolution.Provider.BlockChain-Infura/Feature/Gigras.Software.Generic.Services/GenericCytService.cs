using Gigras.Software.Generic.Repositories;
using System.Linq.Expressions;

namespace Gigras.Software.Generic.Services
{
    public class GenericCytService<T> : IGenericCytService<T> where T : class
    {
        protected readonly IGenericCytRepository<T> _repository;

        public GenericCytService(IGenericCytRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<T> AddAsync(T entity)
        {
            return await _repository.AddAsync(entity);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<T> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            return await _repository.GetByIdAsync(id, include);
        }

        public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object?>>[] includes)
        {
            return await _repository.GetByIdAsync(id, includes);
        }

        public async Task<T?> GetByIdAsync(
               int id,
               Expression<Func<T, bool>>? filter = null,
               params Expression<Func<T, object?>>[] includes)
        {
            return await _repository.GetByIdAsync(id, filter, includes);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await _repository.GetAllAsync(filter);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, Expression<Func<T, object>> orderBy = null, int numrec = -1, int startrec = -1)
        {
            return await _repository.GetAllAsync(filter, orderBy, numrec, startrec);
        }

        public async Task<List<T>> GetAllAsync(
                            Expression<Func<T, bool>> filter = null, // Filtering condition
                            params Expression<Func<T, object>>[] includes // Navigation properties to include
                        )
        {
            return await _repository.GetAllAsync(filter, includes);
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
                                                                        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, // Sorting condition
                                                                        params Expression<Func<T, object>>[] includes
                                                            )
        {
            return await _repository.GetAllAsync(filter, orderBy, includes);
        }

        public async Task UpdateAsync(T entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id, bool IsSave = true)
        {
            await _repository.DeleteAsync(id, IsSave);
        }

        public async Task DeleteManyAsync(List<int> ids, bool IsSave = true)
        {
            await _repository.DeleteManyAsync(ids, IsSave);
        }
        public async Task DeleteByConditionAsync(Func<T, bool> condition, bool IsSave = true)
        {
            await _repository.DeleteByConditionAsync(condition, IsSave);
        }
    }
}