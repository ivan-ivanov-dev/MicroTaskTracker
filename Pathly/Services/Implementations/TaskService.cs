using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Pathly.Data;
using Pathly.Models.DBModels;
using Pathly.Models.ViewModels;
using Pathly.Models.ViewModels.TasksViewModels;
using Pathly.Services.Interfaces;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Pathly.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(TaskCreateViewModel model, string userId)
        {

            var task = new TaskItem
            {
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                CreatedOn = DateTime.UtcNow,
                IsCompleted = false,
                Priority = TaskPriority.Low,
                UserId = userId,
                TaskTags = model.SelectedTagIds.Select(tagId => new TaskTag
                {
                    TagId = tagId

                }).ToList()
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return false;
            }

            if(task.UserId != userId)
            {
                throw new UnauthorizedAccessException();
            }
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TaskListViewModel> GetAllTasksAsync(TaskQueryModel queryModel, string userId)
        {
            var tasksQuery = _context.Tasks
                .Where(u => u.UserId == userId)
                .AsQueryable();

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

        public Task<TaskDetailsViewModel?> GetDetailsAsync(int id, string userId)
        {
            var task = _context.Tasks
                .Where(t => t.Id == id && t.UserId == userId)
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

            if (task == null)
            {
                throw new UnauthorizedAccessException();
            }

            return task;
        }

        public async Task<List<int>> GetTaskTagIdsAsync(int taskId, string userId)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task == null)
            {
                throw new InvalidOperationException("Task not found");
            }

            return await _context.TaskTags
                .Where(tt => tt.TaskId == taskId)
                .Select(tt => tt.TagId)
                .ToListAsync();
        }

        public async Task MarkTaskStatusAsync(int id, string userId)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                throw new InvalidOperationException("Task not found");
            }
            if (task.UserId != userId)
            {
                throw new UnauthorizedAccessException();
            }

            task.IsCompleted = !task.IsCompleted;

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePriorityAsync(int id, TaskPriority priority, string userId)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                throw new InvalidOperationException("Task not found");
            }
            if (task.UserId != userId)
            {
                throw new UnauthorizedAccessException();
            }

            task.Priority =priority;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWithTagsAsync(int id, TaskEditViewModel model, string userId)
        {
            var task = await _context.Tasks
                .Include(t=>t.TaskTags)
                .FirstOrDefaultAsync(t=>t.Id == id );
            if(task == null)
            {
                throw new InvalidOperationException("Task not found");
            }

            if(task.UserId != userId)
            {
                throw new UnauthorizedAccessException();
            }

            task.Title = model.Title;
            task.Description = model.Description;
            task.DueDate = model.DueDate;
            _context.TaskTags.RemoveRange(task.TaskTags);

            task.TaskTags = model.SelectedTagIds.Select(tagId => new TaskTag
            {
                TaskId = task.Id,
                TagId = tagId
            }).ToList();

            await _context.SaveChangesAsync();
        } 
    }
}
