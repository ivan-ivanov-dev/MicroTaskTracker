using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pathly.DataModels;
using Pathly.Services.Contracts;
using Pathly.ViewModels.Roadmaps;
using Pathly.ViewModels.TasksViewModels;

namespace Pathly.Web.Controllers
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
            var model = new RoadmapCreateViewModel { IsEditing = false };

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

            for (int i = 0; i < 3; i++)
            {
                model.Actions.Add(new ActionItemCreateViewModel
                {
                    // Initialize with an empty list so the .Any() check in the View works
                    AssignedTasks = new List<TaskViewModel>()
                });
            }

            return View("RoadmapForm", model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var model = await _roadmapService.GetRoadmapForEditAsync(id, userId);

            if (model == null) return NotFound();

            return View("RoadmapForm", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(RoadmapCreateViewModel model)
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
                return View("RoadmapForm", model);
            }

            try
            {
                var roadmapId = await _roadmapService.SaveRoadmapAsync(model, userId);
                return RedirectToAction("Selection");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while saving the roadmap.");
                return View("RoadmapForm", model);
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
        public async Task<IActionResult> SaveAssignments(int actionId, int roadmapId, string selectedTaskIds)
        {
            var userId = _userManager.GetUserId(User);

            // Parse the comma-separated IDs from the hidden input
            var taskIds = string.IsNullOrEmpty(selectedTaskIds)
                ? new List<int>()
                : selectedTaskIds.Split(',').Select(int.Parse).ToList();

            // Use a loop to call existing service logic for each task
            foreach (var taskId in taskIds)
            {
                await _roadmapService.LinkTaskToActionAsync(taskId, actionId, userId);
            }

            // Redirect back to the details page of the roadmap
            return RedirectToAction("Details", new { id = roadmapId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var success = await _roadmapService.DeleteRoadmapAsync(id, userId);
            if (success)
            {
                return RedirectToAction("Selection");
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Planner(int actionId, int roadmapId)
        {
            var userId = _userManager.GetUserId(User);

            var tasks = await _roadmapService.GetUnlinkedTasksAsync(userId);

            var model = new RoadmapPlannerViewModel
            {
                TargetActionId = actionId,
                RoadmapId = roadmapId,
                UnlinkedTasks = tasks
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlinkTask(int taskId)
        {
            var userId = _userManager.GetUserId(User);
            var success = await _roadmapService.UnlinkTaskFromActionAsync(taskId, userId);
            if(success)
            {
                return Ok();
            }
            return BadRequest();
        }

        // Simple helper class for the JSON data
        public class LinkTaskRequest
        {
            public int TaskId { get; set; }
            public int ActionId { get; set; }
        }
    }
}
