using Microsoft.EntityFrameworkCore;
using SecondTask_WebApp.Models;

namespace SecondTask_WebApp.Data.Repositories
{
    public class AccountClassRepository : BaseRepository<AccountClass>, IAccountClassRepository
    {
        public AccountClassRepository(AppDbContext context) : base(context) { }

        public async Task<List<AccountClass>> GetByFileIdAsync(int fileId)
        {
            return await _dbSet
                .Where(c => c.FileEntityId == fileId)
                .Include(c => c.Accounts)
                    .ThenInclude(a => a.Balance)
                .ToListAsync();
        }
    }
}
