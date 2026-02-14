using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pathly.Data;
using Pathly.DataModels;
using Pathly.Models.ViewModels.TasksViewModels;
using Pathly.Services.Interfaces;
using System.Threading.Tasks;

namespace Pathly.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly ITagService _tagService;
        private readonly UserManager<ApplicationUser> _userManager;
        public TasksController(ITaskService taskService, ITagService tagService, UserManager<ApplicationUser> userManager)
        {
            _taskService = taskService;
            _tagService = tagService;
            _userManager = userManager;
        }

        /*List Tasks*/

        public async Task<IActionResult> Index(TaskQueryModel queryModel)
        {
            var userId = _userManager.GetUserId(User);

            var model = await _taskService.GetAllTasksAsync(queryModel, userId);
            return View(model);
        }

        /*Create Tasks*/

        [HttpGet]
        public async Task<IActionResult> CreateAsync(int? actionId)
        {
            var userId = _userManager.GetUserId(User);
            var tags = await _tagService.GetUserTagsAsync(userId);

            var model = new TaskCreateViewModel
            {
                ActionId = actionId,
                AvailableTags = tags.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList()
            };


            return PartialView("CreatePartialView",model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(TaskCreateViewModel model)
        {
            if (String.IsNullOrWhiteSpace(model.Title))
            {
                ModelState.AddModelError("Title", "The Title field is required.");
            }
            
            if (model.DueDate.HasValue && model.DueDate.Value < DateTime.Now.Date)
            {
                ModelState.AddModelError("DueDate", "Due date cannot be in the past.");
            }

            if(model.SelectedTagIds.Count > 4)
            {
                ModelState.AddModelError("SelectedTagIds", "You can select up to 4 tags.");
            }

            var userId = _userManager.GetUserId(User);

            if (!ModelState.IsValid)
            {
                var tags = await _tagService.GetUserTagsAsync(userId);

                model.AvailableTags = tags.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList();

                return PartialView("CreatePartialView", model);
            }

            try
            {
                await _taskService.CreateAsync(model,userId);
                return Ok();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the task: " + ex.Message);
                return PartialView("CreatePartialView", model);
            }

        }

        /*Edit Tasks*/

        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            var userId = _userManager.GetUserId(User);

            var model = await _taskService.GetDetailsAsync(id,userId);
            if (model == null)
            {
                return NotFound();
            }

            var tags = await _tagService.GetUserTagsAsync(userId);
            var selectedTagIds = await _taskService.GetTaskTagIdsAsync(id, userId);

            var editModel = new TaskEditViewModel
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                SelectedTagIds = selectedTagIds,
                AvailableTags = tags.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList()
            };


            return PartialView("EditPartialView", editModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(TaskEditViewModel model)
        {
            var id = model.Id;
            var userId = _userManager.GetUserId(User);

            if (String.IsNullOrWhiteSpace(model.Title))
            {
                ModelState.AddModelError("Title", "The Title field is required.");
            }

            if (model.DueDate.HasValue && model.DueDate.Value < DateTime.Now)
            {
                ModelState.AddModelError("DueDate", "Due date cannot be in the past.");
            }

            if (model.SelectedTagIds.Count > 4)
            {
                ModelState.AddModelError("SelectedTagIds", "You can select up to 4 tags.");
            }

            if (!ModelState.IsValid)
            {
                var tags = await _tagService.GetUserTagsAsync(userId);

                model.AvailableTags = tags.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList();

                return PartialView("EditPartialView", model);
            }

            await _taskService.UpdateWithTagsAsync(id, model, userId);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Ok(); 
            }

            return RedirectToAction("Index", "Tasks");
        }

        /*Delete Tasks*/

        [HttpGet]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var userId = _userManager.GetUserId(User);

            var task = await _taskService.GetDetailsAsync(id, userId);

            if (task == null)
            {
                return NotFound();
            }

            var model = new TaskDeleteViewModel
            {
                Id = task.Id,
                Title = task.Title
            };

            return PartialView("DeletePartialView", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(TaskDeleteViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var success = await _taskService.DeleteAsync(model.Id, userId);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return success ? Json(new { success = true }) : BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }

        /*View task details Tasks*/

        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var userId = _userManager.GetUserId(User);

            var model = await _taskService.GetDetailsAsync(id, userId);

            if(model == null)
            {
                return NotFound();
            }
            return PartialView("DetailsPartialView", model);
        }

        [HttpPost]
        public async Task<IActionResult> MarkTaskStatus(int id)
        {
            var userId = _userManager.GetUserId(User);

            await _taskService.MarkTaskStatusAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePriority(int id, TaskPriority priority)
        {
            var userId = _userManager.GetUserId(User);

            await _taskService.UpdatePriorityAsync(id,priority, userId);
            return RedirectToAction(nameof(Index));
        }
    }
}
