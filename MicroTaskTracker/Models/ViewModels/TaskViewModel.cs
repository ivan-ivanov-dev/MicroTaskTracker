using MicroTaskTracker.Models.DBModels;

namespace MicroTaskTracker.Models.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public TaskPriority Priority { get; set; }
        public bool IsCompleted { get; set; }
    }
}