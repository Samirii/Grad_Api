namespace Grad_Api.Models.Quiz
{
    public class SubmittedAnswerDto
    {
        public int QuestionId { get; set; } // The ID of the question
        public string SelectedAnswer { get; set; }
    }
}
