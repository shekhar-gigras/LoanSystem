using System.Linq.Expressions;

namespace Gigras.Software.Generic.Repositories
{
    public interface IGenericCytRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        Task<T> AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(int id, bool IsSave = true);
        Task DeleteManyAsync(List<int> ids, bool IsSave = true);
        Task DeleteByConditionAsync(Func<T, bool> condition, bool IsSave = true);

        Task<T?> GetByIdAsync(int id, Func<IQueryable<T>, IQueryable<T>>? include = null);

        Task<T> GetByIdAsync(int id, params Expression<Func<T, object?>>[] includes);

        Task<T?> GetByIdAsync(
                int id,
                Expression<Func<T, bool>>? filter = null,
                params Expression<Func<T, object?>>[] includes);

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter);

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
                                                     Expression<Func<T, object>> orderBy = null, int numrec = -1, int startrec = -1);

        Task<List<T>> GetAllAsync(
                            Expression<Func<T, bool>> filter = null, // Filtering condition
                            params Expression<Func<T, object>>[] includes // Navigation properties to include
                        );

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
                                                                        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, // Sorting condition
                                                                        params Expression<Func<T, object>>[] includes
                                                            );
    }
}