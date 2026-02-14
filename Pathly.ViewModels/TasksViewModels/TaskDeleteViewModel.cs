using System.ComponentModel.DataAnnotations;

namespace Pathly.ViewModels.TasksViewModels
{
    public class TaskDeleteViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
    }
}
