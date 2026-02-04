using MicroTaskTracker.Models.DBModels;
using MicroTaskTracker.Models.ViewModels.TasksViewModels;

namespace MicroTaskTracker.Services.Interfaces
{
    public interface ITaskService
    {
        // Define method signatures for task-related operations
        Task<TaskListViewModel> GetAllTasksAsync(TaskQueryModel queryModel, string userId);
        Task CreateAsync(TaskCreateViewModel model,string userId);
        Task<bool> UpdateAsync(int id, TaskEditViewModel model, string userId);
        Task<bool> DeleteAsync(int id, string userId);
        Task<TaskDetailsViewModel?> GetDetailsAsync(int id, string userId);
        Task MarkTaskStatusAsync(int id, string userId);
        Task UpdatePriorityAsync(int id, TaskPriority priority, string userId);
    }
}
