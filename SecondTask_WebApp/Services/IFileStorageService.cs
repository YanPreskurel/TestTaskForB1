namespace SecondTask_WebApp.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file);
    }
}
