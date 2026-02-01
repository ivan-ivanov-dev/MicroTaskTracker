using System.ComponentModel.DataAnnotations;
namespace MicroTaskTracker.Models.DBModels
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;
        [StringLength(500)]
        public string? Description { get; set; }

        public TaskPriority Priority { get; set; }
        public bool IsCompleted { get; set; }

        public DateTime CreatedOn{ get; set; }
        public DateTime? DueDate { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        [Required]
        public ApplicationUser User { get; set; } = null!;
    }
}
