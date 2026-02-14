using Microsoft.AspNetCore.Mvc.Rendering;
using Pathly.DataModels;
using System.ComponentModel.DataAnnotations;

namespace Pathly.ViewModels.TasksViewModels
{
    public class TaskCreateViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public List<int> SelectedTagIds { get; set; } = new List<int>();

        public int? ActionId { get; set; }
        public ActionItem? Action { get; set; }
        public IEnumerable<SelectListItem> AvailableTags { get; set; } = new List<SelectListItem>();
    }
}
