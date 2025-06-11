namespace Grad_Api.Models.Quiz
{
    public class AddQuestionDto
    {

        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectAnswer { get; set; }
        public string Defficulty { get; set; }
        public string Subject { get; set; }
        public string CourseCategory { get; set; }
        public int UnitNumber { get; set; }
    }
}
