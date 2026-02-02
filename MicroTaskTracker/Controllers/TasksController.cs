using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroTaskTracker.Data;
using MicroTaskTracker.Models.DBModels;
using MicroTaskTracker.Models.ViewModels;
using MicroTaskTracker.Services.Interfaces;
using System.Threading.Tasks;

namespace MicroTaskTracker.Controllers
{
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        public async Task<IActionResult> Index()
        {
            var model = await _taskService.GetAllTasksAsync();
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            /*Implement authentication*/

            await _taskService.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        /*Edit Tasks*/

        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            var model = await _taskService.GetDetailsAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            /*Implement authentication*/
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            /*Implement authentication*/

            await _taskService.UpdateAsync(id, model);
            return RedirectToAction(nameof(Index));
        }

        /*Delete Tasks*/

        [HttpGet]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var task = await _taskService.GetDetailsAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            /*Implement authentication*/

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
            /*Implement authentication*/

            await _taskService.DeleteAsync(model.Id);
            return RedirectToAction(nameof(Index));
        }

        /*View task details Tasks*/

        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            /*Implement authentication*/

            var model = await _taskService.GetDetailsAsync(id);
            return View(model);
        }
    }
}
