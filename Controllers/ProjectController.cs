using Microsoft.AspNetCore.Mvc;

namespace BTL_LTWNC.Controllers
{
    public class ProjectController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Home()
        {
            return View();
        }
    }
}
