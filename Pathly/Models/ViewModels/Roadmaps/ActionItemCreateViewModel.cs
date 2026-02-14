using Pathly.Models.ViewModels.TasksViewModels;
using System.ComponentModel.DataAnnotations;

namespace Pathly.Models.ViewModels.Roadmaps
{
    public class ActionItemCreateViewModel
    {
        public int? Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = null!;

        public string? Resources { get; set; }

        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;

        public List<TaskViewModel> AssignedTasks { get; set; } = new();
    }
}
