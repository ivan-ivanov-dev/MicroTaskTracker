using System.ComponentModel.DataAnnotations;
using Pathly.GCommon;

namespace Pathly.Models.DBModels
{
    public class ActionItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(ValidationConstants.MaxActionItemTitleLength, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = null!;

        [MaxLength(ValidationConstants.MaxActionItemResourcesLength, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Resources { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;


        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;


        [Required]
        public int RoadmapId { get; set; }
        public Roadmap Roadmap { get; set; } = null!;

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
