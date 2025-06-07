namespace Grad_Api.Data
{
    public class QuizScore
    {
        public int Id {  get; set; }

        public int  LessonId { get; set; }

        public string StudentId { get; set; }

        public ApiUser Student { get; set; }
        public Lesson Lesson { get; set; }
        public int Score { get; set; }
    }
}
