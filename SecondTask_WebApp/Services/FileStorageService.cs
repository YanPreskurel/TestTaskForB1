namespace SecondTask_WebApp.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        public FileStorageService(IWebHostEnvironment env) // зависимость для пути прилолежения
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {                               // физический путь к папке wwwroot
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream); // копируем содержимое файла в файловый поток, работает построчной.
            }

            return filePath;
        }
    }
}
