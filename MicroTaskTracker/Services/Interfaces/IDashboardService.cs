using MicroTaskTracker.Models.ViewModels.Dashboard;

namespace MicroTaskTracker.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsViewModel> GetDashboardStatsAsync(int userId);
        Task<DashboardFocusListsViewModel> GetDashboardFocusListsAsync(int userId);
    }
}
