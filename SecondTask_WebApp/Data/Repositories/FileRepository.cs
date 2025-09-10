using Microsoft.EntityFrameworkCore;
using SecondTask_WebApp.Models;

namespace SecondTask_WebApp.Data.Repositories
{
    public class FileRepository : BaseRepository<FileEntity>, IFileRepository
    {
        public FileRepository(AppDbContext context) : base(context) { }

        public async Task<FileEntity?> GetFileWithClassesAsync(int id)
        {
            return await _dbSet
                .Include(f => f.Classes)
                    .ThenInclude(c => c.Accounts)
                        .ThenInclude(a => a.Balance)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
