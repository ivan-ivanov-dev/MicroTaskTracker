using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MicroTaskTracker.Data;
using MicroTaskTracker.Models.DBModels;
using MicroTaskTracker.Models.ViewModels;
using MicroTaskTracker.Models.ViewModels.TasksViewModels;
using MicroTaskTracker.Services.Interfaces;
using System.Threading.Tasks;

namespace MicroTaskTracker.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(TaskCreateViewModel model)
        {
            
            var task = new TaskItem
            {
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                CreatedOn = DateTime.UtcNow,
                IsCompleted = false,
                Priority = TaskPriority.Low,
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return false;
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TaskListViewModel> GetAllTasksAsync(TaskQueryModel queryModel)
        {
            var tasksQuery = _context.Tasks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryModel.SearchByTitle))
            {
                tasksQuery = tasksQuery.Where(t => t.Title.ToLower().Contains(queryModel.SearchByTitle.ToLower()));
            }

            if (queryModel.IsCompleted.HasValue)
            {
                tasksQuery = tasksQuery.Where(t => t.IsCompleted == queryModel.IsCompleted.Value);
            }

            if (queryModel.Priority.HasValue)
            {
                tasksQuery = tasksQuery.Where(t => t.Priority == queryModel.Priority.Value);
            }

            if (queryModel.DueDate.HasValue)
            {
                tasksQuery = tasksQuery.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == queryModel.DueDate.Value.Date);
            }

            if (queryModel.Ascending.HasValue && queryModel.Ascending.Value)
            {
                tasksQuery = tasksQuery.OrderBy(t => t.CreatedOn);
            }
            else
            {
                tasksQuery = tasksQuery.OrderByDescending(t => t.CreatedOn);
            }

            var tasks = await tasksQuery
                .Select(t => new TaskViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    CreatedOn = t.CreatedOn,
                    IsCompleted = t.IsCompleted,
                    Priority = t.Priority
                })
                .ToListAsync();

            var result = new TaskListViewModel
            {
                Tasks = tasks
            };
            return result;
        }

        public Task<TaskDetailsViewModel?> GetDetailsAsync(int id)
        {
            return _context.Tasks
                .Where(t => t.Id == id)
                .Select(t => new TaskDetailsViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    CreatedOn = t.CreatedOn,
                    IsCompleted = t.IsCompleted,
                    Priority = t.Priority
                })
                .FirstOrDefaultAsync();
        }

        public async Task MarkTaskStatusAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                throw new InvalidOperationException("Task not found");
            }

            task.IsCompleted = !task.IsCompleted;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, TaskEditViewModel model)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return false;
            }

            task.Title = model.Title;
            task.Description = model.Description;
            task.DueDate = model.DueDate;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdatePriorityAsync(int id, TaskPriority priority)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                throw new InvalidOperationException("Task not found");
            }

            task.Priority =priority;
            await _context.SaveChangesAsync();
        }
    }
}
