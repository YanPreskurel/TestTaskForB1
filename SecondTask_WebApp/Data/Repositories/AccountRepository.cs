using Microsoft.EntityFrameworkCore;
using SecondTask_WebApp.Models;

namespace SecondTask_WebApp.Data.Repositories
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(AppDbContext context) : base(context) { }

        public async Task<List<Account>> GetByClassIdAsync(int classId)
        {
            return await _dbSet
                .Where(a => a.AccountClassId == classId)
                .Include(a => a.Balance)
                .ToListAsync();
        }
    }
}
