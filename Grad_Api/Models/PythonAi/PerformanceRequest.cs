namespace Grad_Api.Models.PythonAi
{
    public class PerformanceRequest
    {
        public string StudentId { get; set; }
        public int CourseId { get; set; }
        public double AverageScore { get; set; }
        public int NumQuizzesTaken { get; set; }
        public int LastQuizScore { get; set; }
    }

}
