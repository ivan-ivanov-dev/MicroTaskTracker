namespace Pathly.ViewModels.TasksViewModels
{
    public class TaskListViewModel: TaskViewModel
    {
        public IEnumerable<TaskViewModel> Tasks { get; set; } = Enumerable.Empty<TaskViewModel>();
    }
}
