namespace SecondTask_WebApp.ViewModels
{
    public class FileUploadViewModel
    {
        public IFormFile? File { get; set; }
        public bool Preview { get; set; } = false;
    }
}
