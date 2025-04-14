using System.ComponentModel.DataAnnotations;

namespace Grad_Api.Models.Course
{
    public class CourseCreateDto 
        {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3-100 characters")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Teacher name is required")]
        public string? TeacherName { get; set; }

     
        public int CategoryId { get; set; } = 1; // Default category ID
    }
    }


