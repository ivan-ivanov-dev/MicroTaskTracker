using Pathly.Models.ViewModels.TasksViewModels;

namespace Pathly.Models.ViewModels.Roadmaps
{
    public class ActionsDisplayViewModel
    {
        public int ActionId { get; set; }
        public string Title { get; set; }
        public string? Resources { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }

        public List<TaskViewModel> AssignedTasks { get; set; } = new();
    }
}
