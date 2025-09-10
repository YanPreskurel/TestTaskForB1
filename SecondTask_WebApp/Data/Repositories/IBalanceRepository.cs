using SecondTask_WebApp.Models;

namespace SecondTask_WebApp.Data.Repositories
{
    public interface IBalanceRepository : IBaseRepository<Balance>
    {
        Task<List<Balance>> GetByAccountIdsAsync(List<int> accountIds);
    }
}
