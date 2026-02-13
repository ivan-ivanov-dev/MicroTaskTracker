using Microsoft.EntityFrameworkCore;
using MicroTaskTracker.Data;
using MicroTaskTracker.Models.DBModels;
using MicroTaskTracker.Models.ViewModels.Goals;
using MicroTaskTracker.Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MicroTaskTracker.Services.Implementations
{
    public class GoalService : IGoalService
    {
        private readonly ApplicationDbContext _context; 
        public GoalService(ApplicationDbContext context) 
        { 
            _context = context; 
        }
        public async Task CreateAsync(GoalCreateViewModel model, string userId)
        {
            var goal = new Goal
            {
                Title = model.Title,
                ShortDescription = model.ShortDescription,
                TargetDate = model.TargetDate,
                IsActive = model.IsActive,
                UserId = userId
            };

            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.Id == id);
            if (goal == null)
            {
                return false;
            }
            if(goal.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this goal.");
            }

            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<GoalQueryModel> GetAllAsync(GoalQueryModel queryModel, string userId)
        {
            var goalsQuery = _context.Goals
                .Where(g => g.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(queryModel.SearchTerm))
            {
                goalsQuery = goalsQuery.Where(g => g.Title.ToLower().Contains(queryModel.SearchTerm.ToLower()));
            }

            goalsQuery = queryModel.SortOrder switch
            {
                GoalSortOrder.TitleAsc => goalsQuery.OrderBy(g => g.Title),
                GoalSortOrder.TitleDesc => goalsQuery.OrderByDescending(g => g.Title),
                _ => goalsQuery
            };
            var goals = await goalsQuery
                .Select(g => new GoalViewModel
                {
                    Id = g.Id,
                    Title = g.Title,
                    ShortDescription = g.ShortDescription,
                    TargetDate = g.TargetDate,
                    IsActive = g.IsActive,
                    HasRoadmap = _context.Roadmaps.Any(r => r.GoalId == g.Id),
                    RoadmapId = _context.Roadmaps
                        .Where(r => r.GoalId == g.Id)
                        .Select(r => r.Id)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var result = new GoalListViewModel 
            { 
                Goals = goals 
            };

            queryModel.Goals = result;
            return queryModel;

        }

        public Task<GoalDetailsViewModel?> GetDetailsAsync(int id, string userId)
        {
           var goal = _context.Goals
                .Where(g => g.Id == id && g.UserId == userId)
                .Select(g => new GoalDetailsViewModel
                {
                    Id = g.Id,
                    Title = g.Title,
                    ShortDescription = g.ShortDescription,
                    TargetDate = g.TargetDate,
                    IsActive = g.IsActive
                })
                .FirstOrDefaultAsync();

            if (goal == null)
            {
                throw new UnauthorizedAccessException("You do not have permission to view this goal.");
            }

            return goal;
        }

        public async Task UpdateAsync(int id, GoalEditViewModel model, string userId)
        {
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.Id == id);
            if (goal == null)
            {
                throw new InvalidOperationException("Goal not found.");
            }

            if(goal.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to edit this goal.");
            }

            goal.Title = model.Title;
            goal.ShortDescription = model.ShortDescription;
            goal.TargetDate = model.TargetDate;
            goal.IsActive = model.IsActive;

            _context.Goals.Update(goal);
            await _context.SaveChangesAsync();
        }
    }
}
