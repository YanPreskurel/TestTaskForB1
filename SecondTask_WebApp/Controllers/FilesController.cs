using Microsoft.AspNetCore.Mvc;
using SecondTask_WebApp.Data.Repositories;
using SecondTask_WebApp.Services;
using SecondTask_WebApp.ViewModels;

namespace SecondTask_WebApp.Controllers
{
    public class FilesController : Controller
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFileStorageService _fileStorage;
        private readonly IExcelImportService _excelImport;
        private readonly ITableRenderer _tableRenderer;

        public FilesController(
            IFileRepository fileRepository,
            IFileStorageService fileStorage,
            IExcelImportService excelImport,
            ITableRenderer tableRenderer)
        {
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
            _excelImport = excelImport;
            _tableRenderer = tableRenderer;
        }

        // GET: /Files
        public async Task<IActionResult> Index()
        {
            var files = await _fileRepository.GetAllAsync();
            var vm = files.Select(f => new FileViewModel
            {
                Id = f.Id,
                FileName = f.FileName,
                BankName = f.BankName,
                PeriodFrom = f.PeriodFrom == DateTime.MinValue ? null : (DateTime?)f.PeriodFrom,
                PeriodTo = f.PeriodTo == DateTime.MinValue ? null : (DateTime?)f.PeriodTo
            }).ToList();

            return View(vm);
        }

        // GET: /Files/Upload
        [HttpGet]
        public IActionResult Upload()
        {
            return View(new FileUploadViewModel());
        }

        // POST: /Files/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(FileUploadViewModel model)
        {
            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError(nameof(model.File), "Выберите файл для загрузки.");
                return View(model);
            }

            // Сохраняем файл на диск
            var savedPath = await _fileStorage.SaveFileAsync(model.File);

            if (model.Preview)
            {
                // preview: парсим, но не сохраняем
                var (fileInfo, table) = await _excelImport.ParseFileAsync(savedPath, model.File.FileName);
                ViewBag.FileInfo = fileInfo;
                return View("Preview", table);
            }
            else
            {
                var savedEntity = await _excelImport.ImportAsync(savedPath, model.File.FileName);
                return RedirectToAction(nameof(Details), new { id = savedEntity.Id });
            }
        }

        // GET: /Files/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var table = await _tableRenderer.RenderTableAsync(id);
                var file = await _fileRepository.GetByIdAsync(id);

                ViewBag.FileInfo = new FileViewModel
                {
                    Id = file.Id,
                    FileName = file.FileName,
                    BankName = file.BankName,
                    PeriodFrom = file.PeriodFrom == DateTime.MinValue ? null : (DateTime?)file.PeriodFrom,
                    PeriodTo = file.PeriodTo == DateTime.MinValue ? null : (DateTime?)file.PeriodTo
                };

                return View(table);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Не удалось загрузить данные: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }

    // Вспомогательная viewmodel для формы загрузки
    public class FileUploadViewModel
    {
        public IFormFile? File { get; set; }
        public bool Preview { get; set; } = false;
    }
}
