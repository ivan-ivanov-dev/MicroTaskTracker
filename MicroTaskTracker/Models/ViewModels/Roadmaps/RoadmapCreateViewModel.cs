using MicroTaskTracker.Models.ViewModels.Goals;
using System.ComponentModel.DataAnnotations;

namespace MicroTaskTracker.Models.ViewModels.Roadmaps
{
    public class RoadmapCreateViewModel
    {
        public int? SelectedGoalId { get; set; }

        public string? NewGoalTitle { get; set; }
        public string? NewGoalDescription { get; set; }
        public bool NewGoalIsActive { get; set; } = true;
        public DateTime? NewGoalTargetDate { get; set; }


        [MaxLength(2000, ErrorMessage = "This description cannot exceed 2000 characters")]
        public string? Why { get; set; }

        [MaxLength(2000, ErrorMessage = "This description cannot exceed 2000 characters")]
        public string? IdealOutcome { get; set; }

        public List<ActionItemCreateViewModel> Actions { get; set; } = new List<ActionItemCreateViewModel>();
    }
}
