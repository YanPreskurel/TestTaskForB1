using System.Linq.Expressions;

namespace SecondTask_WebApp.Data.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}
