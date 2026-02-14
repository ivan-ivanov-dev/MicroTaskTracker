using Pathly.Models.DBModels;
using System.ComponentModel.DataAnnotations;

namespace Pathly.ViewModels.Roadmaps
{
    public class RoadmapDeatailsViewModel
    {
        public int RoadmapId { get; set; }
        [Required]
        public string GoalTitle { get; set; } = null!;
        public string? GoalDescription { get; set; }
        public string? Why { get; set; }
        public string? IdealOutcome { get; set; }

        public List<ActionsDisplayViewModel> Actions { get; set; } = new();
    }
}
