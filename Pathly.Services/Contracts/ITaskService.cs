using Pathly.DataModels;
using Pathly.Models.ViewModels.TasksViewModels;

namespace Pathly.Services.Interfaces
{
    public interface ITaskService
    {
        // Define method signatures for task-related operations
        Task<TaskListViewModel> GetAllTasksAsync(TaskQueryModel queryModel, string userId);
        Task CreateAsync(TaskCreateViewModel model,string userId);
        Task<bool> DeleteAsync(int id, string userId);
        Task<TaskDetailsViewModel?> GetDetailsAsync(int id, string userId);
        Task MarkTaskStatusAsync(int id, string userId);
        Task UpdatePriorityAsync(int id, TaskPriority priority, string userId);
        Task<List<int>> GetTaskTagIdsAsync(int taskId, string userId);
        Task UpdateWithTagsAsync(int id, TaskEditViewModel model, string userId);
    }
}
