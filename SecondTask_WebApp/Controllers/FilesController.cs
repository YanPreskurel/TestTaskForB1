using Microsoft.AspNetCore.Mvc;

namespace SecondTask_WebApp.Controllers
{
    public class FilesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
