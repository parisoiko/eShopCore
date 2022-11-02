using Microsoft.AspNetCore.Mvc;

namespace ProductsUI.Controllers
{
    public class Dashboard : Controller
    {
        public IActionResult Index() {
            return View();
        }
    }
}
