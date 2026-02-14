using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Pathly.DataModels;
using Pathly.Models.ViewModels.Goals;
using Pathly.Services.Interfaces;
using System.Threading.Tasks;

namespace Pathly.Controllers
{
    [Authorize]
    public class GoalsController : Controller
    {
        private readonly IGoalService _goalService;
        private readonly UserManager<ApplicationUser> _userManager;
        public GoalsController(IGoalService goalService, UserManager<ApplicationUser> userManager)
        {
            _goalService = goalService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(GoalQueryModel queryModel)
        {
            var userId = _userManager.GetUserId(User);
            var goals = await _goalService.GetAllAsync(queryModel ,userId);

            return View(goals);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var model = new GoalCreateViewModel();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(GoalCreateViewModel model)
        {
            var today = DateTime.UtcNow.Date;
            if (String.IsNullOrWhiteSpace(model.Title))
            {
                ModelState.AddModelError("Title", "Title is required.");
            }

            if (model.TargetDate.HasValue && model.TargetDate.Value < today)
            {
                ModelState.AddModelError("TargetDate", "Target date cannot be in the past.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            try
            {
                await _goalService.CreateAsync(model, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the task: " + ex.Message);
                return View(model);
            }

        }

        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            var userId = _userManager.GetUserId(User);
            var goal = await _goalService.GetDetailsAsync(id, userId);
            if (goal == null)
            {
                return NotFound();
            }
            var model = new GoalEditViewModel
            {
                Id = goal.Id,
                Title = goal.Title,
                ShortDescription = goal.ShortDescription,
                TargetDate = goal.TargetDate,
                IsActive = goal.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(GoalEditViewModel model)
        {
            var today = DateTime.UtcNow.Date;
            if (String.IsNullOrWhiteSpace(model.Title))
            {
                ModelState.AddModelError("Title", "Title is required.");
            }
            if (model.TargetDate.HasValue && model.TargetDate.Value < today)
            {
                ModelState.AddModelError("TargetDate", "Target date cannot be in the past.");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userId = _userManager.GetUserId(User);
            try
            {
                await _goalService.UpdateAsync(model.Id, model, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the task: " + ex.Message);
                return View(model);
            }

        }

        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var userId = _userManager.GetUserId(User);
            var goal = await _goalService.GetDetailsAsync(id, userId);
            if (goal == null)
            {
                return NotFound();
            }
            return View(goal);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var userId = _userManager.GetUserId(User);
            await _goalService.DeleteAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }
    }
}
