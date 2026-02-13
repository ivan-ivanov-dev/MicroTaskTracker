using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MicroTaskTracker.Models.DBModels;
using MicroTaskTracker.Models.ViewModels.Roadmaps;
using MicroTaskTracker.Services.Interfaces;
using System.Security.Claims;

namespace MicroTaskTracker.Controllers
{
    [Authorize]
    public class RoadmapController : Controller
    {
        private readonly IRoadmapService _roadmapService;
        private readonly UserManager<ApplicationUser> _userManager;
        public RoadmapController(IRoadmapService roadmapService, UserManager<ApplicationUser> userManager)
        {
            _roadmapService = roadmapService;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Selection()
        {
            var userId = _userManager.GetUserId(User);
            var goals = await _roadmapService.GetAvailableGoalsAsync(userId);
            return View(goals);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? goalId)
        {
            var userId = _userManager.GetUserId(User);
            var model = new RoadmapCreateViewModel();

            for (int i = 0; i < 3; i++)
            {
                model.Actions.Add(new ActionItemCreateViewModel());
            }

            if (goalId.HasValue)
            {
                var goal = await _roadmapService.GetGoalByIdAsync(goalId.Value, userId);

                if (goal != null)
                {
                    model.SelectedGoalId = goal.Id;
                    model.NewGoalTitle = goal.Title;
                    model.NewGoalDescription = goal.ShortDescription;
                    model.NewGoalIsActive = goal.IsActive;
                    model.NewGoalTargetDate = goal.TargetDate;
                }
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoadmapCreateViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (model.SelectedGoalId != null)
            {
                var goal = await _roadmapService.GetGoalByIdAsync(model.SelectedGoalId.Value, userId);
                if (goal == null)
                {
                    ModelState.AddModelError("", "The selected goal does not exist.");
                }
                if (goal.UserId != userId)
                {
                    ModelState.AddModelError("", "You do not have permission to use the selected goal.");
                }
            }
            if (model.Actions == null || model.Actions.Count == 0)
            {
                ModelState.AddModelError("", "Please add at least one action item.");
            }
            if (model.SelectedGoalId == null && string.IsNullOrWhiteSpace(model.NewGoalTitle))
            {
                ModelState.AddModelError("", "Please select an existing goal or enter a new goal title.");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var roadmapId = await _roadmapService.CreateRoadmapAsync(model, userId);
                return RedirectToAction(nameof(Details), new { id = roadmapId });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while saving the roadmap.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            var roadmap = await _roadmapService.GetRoadmapDetailAsync(id, userId);

            if (roadmap == null)
            {
                return NotFound();
            }

            return View(roadmap);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkTask(int taskId, int actionId)
        {
            var userId = _userManager.GetUserId(User);
            var success = await _roadmapService.LinkTaskToActionAsync(taskId, actionId, userId);

            if (success)
            {
                return Json(new { success = true });
            }
            return BadRequest();
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var roadmaps = await _roadmapService.GetAllRoadmapsAsync(userId);
            return View(roadmaps);
        }
    }
}
