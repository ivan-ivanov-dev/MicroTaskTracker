using Microsoft.EntityFrameworkCore;
using Pathly.Data;
using Pathly.DataModels;
using Pathly.Models.DBModels;
using Pathly.Models.ViewModels.Roadmaps;
using Pathly.Models.ViewModels.TasksViewModels;
using Pathly.Services.Interfaces;

namespace Pathly.Services.Implementations
{
    public class RoadmapService : IRoadmapService
    {
        private readonly ApplicationDbContext _context;
        public RoadmapService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> SaveRoadmapAsync(RoadmapCreateViewModel model, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Roadmap roadmap;

                if (model.IsEditing && model.RoadmapId.HasValue)
                {
                    roadmap = await _context.Roadmaps
                        .Include(r => r.Actions)
                        .FirstOrDefaultAsync(r => r.Id == model.RoadmapId && r.UserId == userId);

                    if (roadmap == null)
                    {
                        throw new UnauthorizedAccessException();
                    }

                    roadmap.Why = model.Why;
                    roadmap.IdealOutcome = model.IdealOutcome;

                    var incomingActionIds = model.Actions.Select(a => a.Id).Where(id => id.HasValue).ToList();


                    var actionsToRemove = roadmap.Actions.Where(a => !incomingActionIds.Contains(a.Id)).ToList();
                    _context.Actions.RemoveRange(actionsToRemove);

                    foreach (var actionVm in model.Actions.Where(a => !string.IsNullOrWhiteSpace(a.Title)))
                    {
                        if (actionVm.Id.HasValue && actionVm.Id > 0)
                        {
                            var existingAction = roadmap.Actions.FirstOrDefault(a => a.Id == actionVm.Id);
                            if (existingAction != null)
                            {
                                existingAction.Title = actionVm.Title;
                                existingAction.Resources = actionVm.Resources;
                                existingAction.DueDate = actionVm.DueDate;
                            }
                        }
                        else
                        {
                            _context.Actions.Add(new ActionItem
                            {
                                RoadmapId = roadmap.Id,
                                Title = actionVm.Title,
                                Resources = actionVm.Resources,
                                DueDate = actionVm.DueDate,
                                UserId = userId
                            });
                        }
                    }
                }
                else
                {
                    int goalId = model.SelectedGoalId ?? 0;
                    if (goalId == 0)
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

                    roadmap = new Roadmap
                    {
                        GoalId = goalId,
                        UserId = userId,
                        Why = model.Why,
                        IdealOutcome = model.IdealOutcome
                    };
                    _context.Roadmaps.Add(roadmap);
                    await _context.SaveChangesAsync();

                    foreach (var actionVm in model.Actions.Where(a => !string.IsNullOrWhiteSpace(a.Title)))
                    {
                        _context.Actions.Add(new ActionItem
                        {
                            RoadmapId = roadmap.Id,
                            Title = actionVm.Title,
                            Resources = actionVm.Resources,
                            DueDate = actionVm.DueDate,
                            UserId = userId
                        });
                    }
                }

                await _context.SaveChangesAsync();
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

        public async Task<Goal?> GetGoalByIdAsync(int goalId, string userId)
        {
            var goal =  await _context.Goals.FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);
            if(goal == null)
            {
                throw new UnauthorizedAccessException();
            }
            return goal;
        }

        public async Task<RoadmapDeatailsViewModel?> GetRoadmapDetailAsync(int roadmapId, string userId)
        {
            var roadmap = await _context.Roadmaps
                .Include(r => r.Goal)
                .Include(r => r.Actions)
                    .ThenInclude(a => a.Tasks)
                        .ThenInclude(t => t.TaskTags)
                            .ThenInclude(tt => tt.Tag)
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

        public async Task<RoadmapCreateViewModel?> GetRoadmapForEditAsync(int roadmapId, string userId)
        {
            var roadmap = await _context.Roadmaps
                .Include(r => r.Goal)
                .Include(r => r.Actions)
                    .ThenInclude(a => a.Tasks)
                        .ThenInclude(t => t.TaskTags)
                            .ThenInclude(tt => tt.Tag)
                .FirstOrDefaultAsync(r => r.Id == roadmapId && r.UserId == userId);

            if (roadmap == null) return null;

            return new RoadmapCreateViewModel
            {
                IsEditing = true,
                RoadmapId = roadmap.Id,
                SelectedGoalId = roadmap.GoalId,
                NewGoalTitle = roadmap.Goal.Title,
                NewGoalDescription = roadmap.Goal.ShortDescription,
                Why = roadmap.Why,
                IdealOutcome = roadmap.IdealOutcome,
                Actions = roadmap.Actions.Select(a => new ActionItemCreateViewModel
                {
                    Id = a.Id, // CRITICAL: This keeps tasks safe
                    Title = a.Title,
                    Resources = a.Resources,
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
                }).ToList()
            };
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
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<TaskItem>> GetUnlinkedTasksAsync(string userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId && t.ActionId == null)
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();
        }

        public async Task<bool> UnlinkTaskFromActionAsync(int taskId, string userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task == null)
            {
                return false;
            }

            if(task.UserId != userId)
            {
                throw new UnauthorizedAccessException();
            }

            task.ActionId = null; // Remove the link
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
