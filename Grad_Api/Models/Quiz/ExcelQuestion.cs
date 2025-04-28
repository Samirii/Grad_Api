namespace Grad_Api.Models.Quiz
{
    public class ExcelQuestion
    {
        public int LessonId { get; set; }   
        
        public string QuestionTitle { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
    }
}