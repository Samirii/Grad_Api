using System.ComponentModel.DataAnnotations;

namespace Grad_Api.Models.Enrollment
{
    public class EnrollmentCreateDto
    {
        [Required]
        public string StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }
    }
} 