using System.ComponentModel.DataAnnotations;

namespace MicroTaskTracker.Models.DBModels
{
    public class ActionItem
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = null!;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public int GoalId { get; set; }
        public Goal Goal { get; set; } = null!;

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
