using MicroTaskTracker.Models.ViewModels.Dashboard;
using MicroTaskTracker.Services.Interfaces;

namespace MicroTaskTracker.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        public Task<DashboardFocusListsViewModel> GetDashboardFocusListsAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<DashboardStatsViewModel> GetDashboardStatsAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
