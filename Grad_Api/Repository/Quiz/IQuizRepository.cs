using Grad_Api.Data;
using Grad_Api.Models;
using Grad_Api.Models.Quiz;

namespace Grad_Api.Repository
{
    public interface IQuizRepository
    {
        Task<Quiz> GetQuizByLessonIdAsync(int lessonId);
        Task<Quiz> GetQuizWithQuestionsByLessonIdAsync(int lessonId);
        Task<Quiz> AddQuizAsync(Quiz quiz);
        Task AddQuestionsAsync(IEnumerable<Question> questions);
        Task<IEnumerable<Question>> GetQuestionsByQuizIdAsync(int quizId);
        Task<List<Question>> GetRandomQuestionsForQuizAsync(int quizId, int count);
        Task SaveQuizScoreAsync(QuizScore quizScore);
        Task<List<QuizScore>> GetQuizScoresForStudentAsync(string studentId);

    }
}
