using Grad_Api.Data;

namespace Grad_Api.Models.Lessons
{
    public class ReadLessonDto:BaseDto
    {
 
        public string? Title { get; set; }

        public string? VideoUrl { get; set; }

        public string? Content { get; set; }

        public int? CourseId { get; set; }
        public bool HasQuiz { get; set; }
    }
}

