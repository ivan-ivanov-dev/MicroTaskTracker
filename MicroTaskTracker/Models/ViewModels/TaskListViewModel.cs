namespace MicroTaskTracker.Models.ViewModels
{
    public class TaskListViewModel
    {
        public IEnumerable<TaskViewModel> Tasks { get; set; } = Enumerable.Empty<TaskViewModel>();
    }
}
