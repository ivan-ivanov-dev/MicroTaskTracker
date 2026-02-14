using Pathly.DataModels;

namespace Pathly.Models.ViewModels.Roadmaps
{
    // This view model is used to display the roadmap planner page,
    // where users can see unlinked tasks and link them to actions.
    public class RoadmapPlannerViewModel
    {
        public int TargetActionId { get; set; }
        public int RoadmapId { get; set; } // So we can return to the right roadmap
        public IEnumerable<TaskItem> UnlinkedTasks { get; set; }
    }
}
