namespace Grad_Api.Models.Quiz
{
    public class SubmitAnswersDto
    {
        public int QuizId { get; set; } // The quiz for which answers are being submitted

        public List<SubmittedAnswerDto> Answers { get; set; } = new List<SubmittedAnswerDto>();
    }
}
