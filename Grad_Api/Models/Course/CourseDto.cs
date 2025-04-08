using System.ComponentModel.DataAnnotations;

namespace Grad_Api.Models.Course
{
    public class CourseDto
    {
        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(50)]
        public string? ThumbnailUrl { get; set; }

        public int? CategoryId { get; set; }
    }
} 