using MicroTaskTracker.Models.DBModels;
using MicroTaskTracker.Models.ViewModels.TasksViewModels;

namespace MicroTaskTracker.Services.Interfaces
{
    public interface ITaskService
    {
        // Define method signatures for task-related operations
        Task<TaskListViewModel> GetAllTasksAsync(TaskQueryModel queryModel);
        Task CreateAsync(TaskCreateViewModel model);
        Task<bool> UpdateAsync(int id, TaskEditViewModel model);
        Task<bool> DeleteAsync(int id);
        Task<TaskDetailsViewModel?> GetDetailsAsync(int id);
        Task MarkTaskStatusAsync(int id);
        Task UpdatePriorityAsync(int id, TaskPriority priority);
    }
}
