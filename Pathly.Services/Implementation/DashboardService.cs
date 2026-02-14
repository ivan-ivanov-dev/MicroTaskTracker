using Microsoft.EntityFrameworkCore;
using Pathly.Data;
using Pathly.Models.DBModels;
using Pathly.Models.ViewModels.Dashboard;
using Pathly.Services.Interfaces;

namespace Pathly.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;
        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<DashboardFocusListsViewModel> GetDashboardFocusListsAsync(string userId)
        {
            var today = DateTime.UtcNow.Date;

            var userTasks = _context.Tasks.Where(t => t.UserId == userId);

            var dueTodayTasks = await userTasks
                .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == today)
                .OrderBy(t => t.DueDate)
                .Take(5)
                .Select(t => new TaskSummaryViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    DueDate = t.DueDate,
                    IsCompleted = t.IsCompleted,
                    Priority = t.Priority
                })
                .ToListAsync();
            var overdueTasks = await userTasks
                .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date < today && !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .Take(5)
                .Select(t => new TaskSummaryViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    DueDate = t.DueDate,
                    IsCompleted = t.IsCompleted,
                    Priority = t.Priority
                })
                .ToListAsync();

            var futureHighPriorityTasks = await userTasks
                .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date > today && (t.Priority == TaskPriority.High || t.Priority == TaskPriority.Critical))
                .OrderBy(t => t.DueDate)
                .Take(5)
                .Select(t => new TaskSummaryViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    DueDate = t.DueDate,
                    IsCompleted = t.IsCompleted,
                    Priority = t.Priority
                })
                .ToListAsync();
            return new DashboardFocusListsViewModel
            {
                DueTodayTasks = dueTodayTasks,
                OverdueTasks = overdueTasks,
                FutureHighPriorityTasks = futureHighPriorityTasks
            };
        }

        public async Task<DashboardStatsViewModel> GetDashboardStatsAsync(string userId)
        {
            var today = DateTime.UtcNow.Date;

            var userTasks = _context.Tasks.Where(t => t.UserId == userId);

            var totalTasks = await userTasks.CountAsync();
            var completedTasks = await userTasks.CountAsync(t => t.IsCompleted);

            var totalTasksToday = await userTasks.CountAsync(t => t.DueDate.HasValue && t.DueDate.Value == today);
            var completedTasksToday = await userTasks.CountAsync(t => t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value == today);

            return new DashboardStatsViewModel
            {
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                TotalTasksDueToday = totalTasksToday,
                CompletedTasksDueToday = completedTasksToday
            };
        }
    }
}
