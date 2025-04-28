namespace Grad_Api.Data
{
    public class QuizScore
    {
        public int id {  get; set; }

        public int  QuizId { get; set; }

        public string StudentId { get; set; }

        public ApiUser Student { get; set; }
        public Quiz Quiz { get; set; }
        public int Score { get; set; }
    }
}
