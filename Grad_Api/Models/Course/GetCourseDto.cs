namespace Grad_Api.Models.Course
{
    public class GetCourseDto : CourseDto
    {
        public int Id { get; set; }
        public string? CategoryName { get; set; }
        public string? InstructorName { get; set; }
        public int LessonCount { get; set; }
        public int QuizCount { get; set; }
    }
} 