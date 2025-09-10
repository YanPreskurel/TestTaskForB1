using SecondTask_WebApp.Models;

namespace SecondTask_WebApp.Data.Repositories
{
    public interface IFileRepository : IBaseRepository<FileEntity>
    {
        Task<FileEntity?> GetFileWithClassesAsync(int id);
    }
}
