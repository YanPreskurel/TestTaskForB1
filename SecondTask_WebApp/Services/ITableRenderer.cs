using SecondTask_WebApp.ViewModels;

namespace SecondTask_WebApp.Services
{
    public interface ITableRenderer
    {
        Task<TableViewModel> BuildAsync(int fileId, CancellationToken ct = default);
    }
}
