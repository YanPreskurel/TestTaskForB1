using SecondTask_WebApp.Models;

namespace SecondTask_WebApp.Data.Repositories
{
    public interface IAccountClassRepository : IBaseRepository<AccountClass>
    {
        Task<List<AccountClass>> GetByFileIdAsync(int fileId);
    }
}
