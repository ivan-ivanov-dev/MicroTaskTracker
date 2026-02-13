using MicroTaskTracker.Models.DBModels;
using MicroTaskTracker.Models.ViewModels.Roadmaps;

namespace MicroTaskTracker.Services.Interfaces
{
    public interface IRoadmapService
    {
        // Retrieval
        Task<IEnumerable<Goal>> GetAvailableGoalsAsync(string userId);
        Task<RoadmapDeatailsViewModel?> GetRoadmapDetailAsync(int roadmapId, string userId);
        Task<List<Roadmap>> GetAllRoadmapsAsync(string userId);

        // Creation
        Task<int> CreateRoadmapAsync(RoadmapCreateViewModel model, string userId);

        // Management
        Task<bool> LinkTaskToActionAsync(int taskId, int actionId, string userId);
        Task<bool> DeleteRoadmapAsync(int roadmapId, string userId);
    }
}
