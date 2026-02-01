using MicroTaskTracker.Models.DBModels;
using System.ComponentModel.DataAnnotations;

namespace MicroTaskTracker.Models.ViewModels
{
    public class TaskCreateViewModel
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }
        public TaskPriority Priority { get; set; }
    }
}
