using System.ComponentModel.DataAnnotations;

namespace MicroTaskTracker.Models.ViewModels.TasksViewModels
{
    public class TaskDeleteViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
    }
}
