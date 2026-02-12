using System.ComponentModel.DataAnnotations;

namespace MicroTaskTracker.Models.ViewModels.Goals
{
    public class GoalViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters")]
        public string Title { get; set; } = null!;
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }
        public DateTime? TargetDate { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
