using System.ComponentModel.DataAnnotations;

namespace Pathly.Models.DBModels
{
    public class Goal
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters")]
        public string Title { get; set; } = null!;

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? ShortDescription { get; set; }
        public DateTime? TargetDate { get; set; }
        public bool IsActive { get; set; } = true;

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

    }
}
