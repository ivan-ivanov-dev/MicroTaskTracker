using Pathly.GCommon;
using System.ComponentModel.DataAnnotations;
namespace Pathly.Models.DBModels
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(ValidationConstants.MaxTaskItemTitleLength,ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = null!;

        [MaxLength(ValidationConstants.MaxTaskItemDescriptionLength,ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Low;
        public bool IsCompleted { get; set; }

        public DateTime CreatedOn{ get; set; }
        public DateTime? DueDate { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        public ApplicationUser? User { get; set; }
        public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();

        public int? ActionId { get; set; }
        public ActionItem? Action { get; set; }
    }
}
