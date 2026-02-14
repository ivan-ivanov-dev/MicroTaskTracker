using Pathly.Models.DBModels;
using System.ComponentModel.DataAnnotations;

namespace Pathly.Models.ViewModels.Dashboard
{
    public class TaskSummaryViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        public DateTime? DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public bool IsCompleted { get; set; }
    }
}
