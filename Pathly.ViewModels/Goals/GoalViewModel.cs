using System.ComponentModel.DataAnnotations;

namespace Pathly.ViewModels.Goals
{
    public class GoalViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters")]
        public string Title { get; set; } = null!;
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? ShortDescription { get; set; }
        public DateTime? TargetDate { get; set; }
        public bool IsActive { get; set; } = true;

        public bool HasRoadmap { get; set; }
        public int? RoadmapId { get; set; }

    }
}
