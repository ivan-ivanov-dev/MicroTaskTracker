using MicroTaskTracker.Models.ViewModels.Dashboard;
using System.IO.Compression;

namespace MicroTaskTracker.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsViewModel> GetDashboardStatsAsync(string userId);
        Task<DashboardFocusListsViewModel> GetDashboardFocusListsAsync(string userId);
    }
}
