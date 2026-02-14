using Pathly.DataModels;
using System.ComponentModel.DataAnnotations;

namespace Pathly.Models.ViewModels.TasksViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Low;
        public bool IsCompleted { get; set; }
        
        public List<string> Tags { get; set; } = new List<string>();
    }
}