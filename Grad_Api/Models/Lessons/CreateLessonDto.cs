using System.ComponentModel.DataAnnotations;

namespace Grad_Api.Models.Lessons
{
    public class CreateLessonDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; } = string.Empty;

        public string? VideoUrl { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "CourseId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "CourseId must be greater than 0")]
        public int CourseId { get; set; }
    }
}
