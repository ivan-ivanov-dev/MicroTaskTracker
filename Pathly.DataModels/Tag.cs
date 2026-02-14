using Microsoft.EntityFrameworkCore;
using Pathly.GCommon;
using System.ComponentModel.DataAnnotations;

namespace Pathly.Models.DBModels
{
    public class Tag
    {
        
        
        public int Id { get; set; }
        [Required]
        [MaxLength(ValidationConstants.MaxTagNameLength,ErrorMessage ="Tag name cannot exceed 30 characters")]
        public string Name { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    }
}
