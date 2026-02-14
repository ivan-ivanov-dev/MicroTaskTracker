using Pathly.Models.DBModels;
using Pathly.Models.ViewModels.Roadmaps;

namespace Pathly.Services.Interfaces
{
    public interface IRoadmapService
    {
        // Retrieval
        Task<IEnumerable<Goal>> GetAvailableGoalsAsync(string userId);
        Task<RoadmapDeatailsViewModel?> GetRoadmapDetailAsync(int roadmapId, string userId);
        Task<List<Roadmap>> GetAllRoadmapsAsync(string userId);
        Task<Goal?> GetGoalByIdAsync(int goalId, string userId);

        // Create/Edit
        Task<int> SaveRoadmapAsync(RoadmapCreateViewModel model, string userId);
        Task<RoadmapCreateViewModel?> GetRoadmapForEditAsync(int roadmapId, string userId);


        // Management
        Task<bool> LinkTaskToActionAsync(int taskId, int actionId, string userId);
        Task<IEnumerable<TaskItem>> GetUnlinkedTasksAsync(string userId);
        Task<bool> UnlinkTaskFromActionAsync(int taskId, string userId);
        Task<bool> DeleteRoadmapAsync(int roadmapId, string userId);
    }
}
