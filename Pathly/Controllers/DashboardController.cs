using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pathly.Models.ViewModels.Dashboard;
using Pathly.Services.Interfaces;
using System.Security.Claims;

namespace Pathly.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var stats = await _dashboardService.GetDashboardStatsAsync(userId);
            var focusLists = await _dashboardService.GetDashboardFocusListsAsync(userId);

            var model = new DashboardViewModel
            {
                Stats = stats,
                FocusLists = focusLists
            };
            return View(model);
        }
    }
}
