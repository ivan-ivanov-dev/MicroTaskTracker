using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroTaskTracker.Data;
using MicroTaskTracker.Models.DBModels;
using MicroTaskTracker.Models.ViewModels;
using System.Threading.Tasks;

namespace MicroTaskTracker.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var tasks =await _context.Tasks.Select(t => new TaskViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                CreatedOn = t.CreatedOn,
                IsCompleted = t.IsCompleted,
                Priority = t.Priority
            }).ToListAsync();

            var model = new TaskListViewModel
            {
                Tasks = tasks
            };

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

            var task = new TaskItem
            {
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                CreatedOn = DateTime.UtcNow,
                IsCompleted = false,
                Priority = model.Priority
            };

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /*Edit Tasks*/

        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            var tasks = await _context.Tasks.ToListAsync();
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            /*Implement authentication*/

            var model = new TaskEditViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditAsync(int id, TaskEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            /*Implement authentication*/

            task.Title = model.Title;
            task.Description = model.Description;
            task.DueDate = model.DueDate;

            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /*Delete Tasks*/

        [HttpGet]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            /*Implement authentication*/

            var model = new TaskDeleteViewModel
            {
                Id = task.Id,
                Title = task.Title
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(TaskDeleteViewModel model)
        {
            
            var task = await _context.Tasks.FindAsync(model.Id);
            if (task == null)
            {
                return NotFound();
            }

            /*Implement authentication*/

             _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /*View task details Tasks*/

        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            
            /*Implement authentication*/

            var taskModel = new TaskViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                CreatedOn = task.CreatedOn,
                IsCompleted = task.IsCompleted,
                Priority = task.Priority
            };

            var model = new TaskDetailsViewModel
            {
                Task = taskModel
            };

            return View(model);
        }
    }
}
