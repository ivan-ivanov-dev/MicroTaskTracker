using System.ComponentModel.DataAnnotations;
namespace MicroTaskTracker.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedOn{ get; set; }

        public DateTime? DueDate { get; set; }
    }
}
