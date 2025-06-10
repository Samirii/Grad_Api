using System.ComponentModel.DataAnnotations;

namespace Grad_Api.Models.PythonAi
{
    public class StudentPerformanceDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }

        [Range(0, 100)]
        public double PredictedScore { get; set; }

        [Range(0, 100)]
        public double LastQuizScore { get; set; }

        public int QuizCount { get; set; }

        [Range(0, 100)]
        public double AverageScore { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}