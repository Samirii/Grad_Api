using Grad_Api.Data;
using Grad_Api.Models.Quiz;

namespace Grad_Api.Services
{
    public interface IQuizService
    {
        Task ImportQuestionsFromExcelAsync(string filePath);
        Task<IEnumerable<Grad_Api.Data.Question>> GetQuestionsByQuizIdAsync(int quizId);
        Task<Quiz> GetQuizWithQuestionsByLessonIdAsync(int lessonId);
        Task<List<Grad_Api.Data.Question>> GetRandomQuestionsForQuizAsync(int quizId, int count);
        Task<ScoreResponseDto> CalculateAndSaveScoreAsync(string studentId, SubmitAnswersDto submitAnswersDto);
        Task<List<QuizScoreDto>> GetQuizScoresForStudentAsync(string studentId);
        Task<QuizScoreDto> SendScoreAsync(string studentId, int quizId, int score); 

    }
}
