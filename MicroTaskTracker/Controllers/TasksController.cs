using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroTaskTracker.Data;
using MicroTaskTracker.Models.DBModels;
using MicroTaskTracker.Models.ViewModels.TasksViewModels;
using MicroTaskTracker.Services.Interfaces;
using System.Threading.Tasks;

namespace MicroTaskTracker.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly UserManager<ApplicationUser> _userManager;
        public TasksController(ITaskService taskService, UserManager<ApplicationUser> userManager)
        {
            _taskService = taskService;
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
        public IActionResult Create()
        {
            var model = new TaskCreateViewModel();
            return View(model);
        }
        [HttpPost]
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

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            try
            {
                await _taskService.CreateAsync(model,userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the task: " + ex.Message);
                return View(model);
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

            var editModel = new TaskEditViewModel
            {
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
            };


            return View(editModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditAsync(int id, TaskEditViewModel model)
        {
            var userId = _userManager.GetUserId(User);

            if (String.IsNullOrWhiteSpace(model.Title))
            {
                ModelState.AddModelError("Title", "The Title field is required.");
            }

            if (model.DueDate.HasValue && model.DueDate.Value < DateTime.Now)
            {
                ModelState.AddModelError("DueDate", "Due date cannot be in the past.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _taskService.UpdateAsync(id, model, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while editing the task: " + ex.Message);
                return View(model);
            }
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
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(TaskDeleteViewModel model)
        {
            var userId = _userManager.GetUserId(User);

            await _taskService.DeleteAsync(model.Id, userId);
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
            return View(model);
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
