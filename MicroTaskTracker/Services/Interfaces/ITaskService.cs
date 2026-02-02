using MicroTaskTracker.Models.ViewModels;

namespace MicroTaskTracker.Services.Interfaces
{
    public interface ITaskService
    {
        // Define method signatures for task-related operations
        Task<TaskListViewModel> GetAllTasksAsync();
        Task CreateAsync(TaskCreateViewModel model);
        Task<bool> UpdateAsync(int id, TaskEditViewModel model);
        Task<bool> DeleteAsync(int id);
        Task<TaskDetailsViewModel?> GetDetailsAsync(int id);

    }
}
