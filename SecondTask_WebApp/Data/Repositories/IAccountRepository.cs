using SecondTask_WebApp.Models;

namespace SecondTask_WebApp.Data.Repositories
{
    public interface IAccountRepository : IBaseRepository<Account>
    {
        Task<List<Account>> GetByClassIdAsync(int classId);
    }
}
