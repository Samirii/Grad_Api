namespace Grad_Api.Models.Course
{
    public class CourseUpdateDto 
    {
        public string? Title { get; set; }

        public string? Description { get; set; }
        public string? TeacherName { get; set; }      
        public int CategoryId { get; set; }

    }
}
