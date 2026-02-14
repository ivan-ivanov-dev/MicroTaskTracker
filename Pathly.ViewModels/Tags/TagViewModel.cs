using System.ComponentModel.DataAnnotations;

namespace Pathly.ViewModels.Tags
{
    public class TagViewModel
    {
        [Required]
        [StringLength(30, ErrorMessage = "Tag name cannot exceed 30 characters")]
        public string Name { get; set; } = null!;
    }
}
