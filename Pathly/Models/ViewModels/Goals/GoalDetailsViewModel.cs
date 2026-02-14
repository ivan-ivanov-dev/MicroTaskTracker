using Pathly.Models.ViewModels.Roadmaps;
using System.ComponentModel.DataAnnotations;

namespace Pathly.Models.ViewModels.Goals
{
    public class GoalDetailsViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters")]
        public string Title { get; set; } = null!;
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? ShortDescription { get; set; }
        public DateTime? TargetDate { get; set; }
        public bool IsActive { get; set; }
        public List<ActionsDisplayViewModel> Actions { get; set; } = new();
    }
}
