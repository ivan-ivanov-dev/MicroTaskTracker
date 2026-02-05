namespace MicroTaskTracker.Models.ViewModels.Dashboard
{
    public class DashboardFocusListsViewModel
    {
        public IEnumerable<TaskSummaryViewModel> DueTodayTasks { get; set; } = new List<TaskSummaryViewModel>();
        public IEnumerable<TaskSummaryViewModel> FutureHighPriorityTasks { get; set; } = new List<TaskSummaryViewModel>();
        public IEnumerable<TaskSummaryViewModel> OverdueTasks { get; set; } = new List<TaskSummaryViewModel>();
    }
}
