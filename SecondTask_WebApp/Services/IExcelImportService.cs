using SecondTask_WebApp.Models;
using SecondTask_WebApp.ViewModels;

namespace SecondTask_WebApp.Services
{
    public interface IExcelImportService
    {
        Task<FileEntity> ImportAsync(string filePath, string fileName);

        Task<(FileViewModel FileInfo, TableViewModel Table)> ParseFileAsync(string filePath, string fileName);
    }
}
