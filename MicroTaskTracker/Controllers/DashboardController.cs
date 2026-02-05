using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroTaskTracker.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index(string id)
        {
            return View();
        }
    }
}
