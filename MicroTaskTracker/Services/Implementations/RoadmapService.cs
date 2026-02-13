using Microsoft.EntityFrameworkCore;
using MicroTaskTracker.Data;
using MicroTaskTracker.Models.DBModels;
using MicroTaskTracker.Models.ViewModels.Roadmaps;
using MicroTaskTracker.Models.ViewModels.TasksViewModels;
using MicroTaskTracker.Services.Interfaces;

namespace MicroTaskTracker.Services.Implementations
{
    public class RoadmapService : IRoadmapService
    {
        private readonly ApplicationDbContext _context;
        public RoadmapService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> CreateRoadmapAsync(RoadmapCreateViewModel model, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int goalId;

                // 1. Logic for New vs Existing Goal
                if (model.SelectedGoalId.HasValue && model.SelectedGoalId.Value > 0)
                {
                    goalId = model.SelectedGoalId.Value;
                }
                else
                {
                    var newGoal = new Goal
                    {
                        Title = model.NewGoalTitle!,
                        ShortDescription = model.NewGoalDescription,
                        UserId = userId,
                        IsActive = true
                    };
                    _context.Goals.Add(newGoal);
                    await _context.SaveChangesAsync();
                    goalId = newGoal.Id;
                }

                // 2. Create the Roadmap entry
                var roadmap = new Roadmap
                {
                    GoalId = goalId,
                    UserId = userId,
                    Why = model.Why,
                    IdealOutcome = model.IdealOutcome
                };
                _context.Roadmaps.Add(roadmap);
                await _context.SaveChangesAsync();

                // 3. Add the initial Action Items
                if (model.Actions != null && model.Actions.Any())
                {
                    foreach (var actionVm in model.Actions)
                    {
                        if (string.IsNullOrWhiteSpace(actionVm.Title)) continue;

                        var action = new ActionItem
                        {
                            RoadmapId = roadmap.Id,
                            Title = actionVm.Title,
                            Resources = actionVm.Resources,
                            DueDate = actionVm.DueDate,
                            UserId = userId,
                            IsCompleted = false
                        };
                        _context.Actions.Add(action);
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return roadmap.Id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        

        public async Task<bool> DeleteRoadmapAsync(int roadmapId, string userId)
        {
            var roadmap = await _context.Roadmaps
                .FirstOrDefaultAsync(r => r.Id == roadmapId && r.UserId == userId);

            if (roadmap == null)
            {
                return false;
            }

            if(roadmap.UserId != userId)
            {
                throw new UnauthorizedAccessException();
            }

            _context .Roadmaps.Remove(roadmap);
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<List<Roadmap>> GetAllRoadmapsAsync(string userId)
        {
            var roadmaps = _context.Roadmaps
                .Where(r => r.UserId == userId)
                .Include(r => r.Goal)
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            if(roadmaps == null)
            {
                throw new UnauthorizedAccessException();
            }
            return roadmaps;
        }

        public async Task<IEnumerable<Goal>> GetAvailableGoalsAsync(string userId)
        {
            return await _context.Goals
                .Where(g => g.UserId == userId && !_context.Roadmaps.Any(r => r.GoalId == g.Id))
                .OrderByDescending(g => g.Id)
                .ToListAsync();
        }

        public async Task<RoadmapDeatailsViewModel?> GetRoadmapDetailAsync(int roadmapId, string userId)
        {
            var roadmap = await _context.Roadmaps
                .Where(r => r.Id == roadmapId && r.UserId == userId)
                .Select(r => new RoadmapDeatailsViewModel
                {
                    RoadmapId = r.Id,
                    GoalTitle = r.Goal.Title,
                    GoalDescription = r.Goal.ShortDescription,
                    Why = r.Why,
                    IdealOutcome = r.IdealOutcome,
                    Actions = r.Actions.Select(a => new ActionsDisplayViewModel
                    {
                        ActionId = a.Id,
                        Title = a.Title,
                        Resources = a.Resources,
                        IsCompleted = a.IsCompleted,
                        DueDate = a.DueDate,
                        AssignedTasks = a.Tasks
                            .OrderBy(t => t.CreatedOn)
                            .Select(t => new TaskViewModel
                            {
                                Id = t.Id,
                                Title = t.Title,
                                IsCompleted = t.IsCompleted,
                                Priority = t.Priority,
                                CreatedOn = t.CreatedOn, 
                                Tags = t.TaskTags.Select(tt => tt.Tag.Name).ToList()
                            }).ToList()
                    }).OrderBy(a => a.DueDate).ToList()
                }).FirstOrDefaultAsync();

            if(roadmap == null)
            {
                throw new UnauthorizedAccessException();
            }

            return roadmap;
        }

        public async Task<bool> LinkTaskToActionAsync(int taskId, int actionId, string userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
            var actionExists = await _context.Actions.AnyAsync(a => a.Id == actionId && a.UserId == userId);

            if (task == null || !actionExists)
            {
                return false;
            }

            task.ActionId = actionId;
            _context.Entry(task).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
