using Microsoft.EntityFrameworkCore;
using SecondTask_WebApp.Models;

namespace SecondTask_WebApp.Data.Repositories
{
    public class BalanceRepository : BaseRepository<Balance>, IBalanceRepository
    {
        public BalanceRepository(AppDbContext context) : base(context) { }

        public async Task<List<Balance>> GetByAccountIdsAsync(List<int> accountIds)
        {
            return await _dbSet
                .Where(b => accountIds.Contains(b.AccountId))
                .ToListAsync();
        }
    }
}
