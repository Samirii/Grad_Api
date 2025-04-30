using Grad_Api.Data;
using Grad_Api.Models;
using Grad_Api.Models.Quiz;

namespace Grad_Api.Repository
{
    public interface IQuizRepository
    {
        Task<Quiz> GetQuizByQuizIdAsync(int quizId);
        Task<Quiz> AddQuizAsync(Quiz quiz);
        Task AddQuestionsAsync(IEnumerable<Question> questions);
        Task<IEnumerable<Question>> GetQuestionsByQuizIdAsync(int quizId);
        Task<List<Question>> GetRandomQuestionsForQuizAsync(int quizId, int count);
        Task SaveQuizScoreAsync(QuizScore quizScore);
        Task<List<QuizScore>> GetQuizScoresForStudentAsync(string studentId);
        Task<List<Question>> GetQuestionsByUnitAndCategoryAsync(int unitNumber, string courseName, string categoryName);



    }
}
