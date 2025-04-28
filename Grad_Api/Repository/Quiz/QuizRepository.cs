using Grad_Api.Data;
using Grad_Api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Grad_Api.Repository
{
    public class QuizRepository : IQuizRepository
    {
        private readonly GradProjDbContext _context;

        public QuizRepository(GradProjDbContext context)
        {
            _context = context;
        }

        public async Task<Quiz> GetQuizByLessonIdAsync(int lessonId)
        {
            return await _context.Quizzes
                .FirstOrDefaultAsync(q => q.LessonId == lessonId);
        }

        public async Task<Quiz> GetQuizWithQuestionsByLessonIdAsync(int lessonId)
        {
            return await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.LessonId == lessonId);
        }

        public async Task<Quiz> AddQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task AddQuestionsAsync(IEnumerable<Question> questions)
        {
            await _context.Questions.AddRangeAsync(questions);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Question>> GetQuestionsByQuizIdAsync(int quizId)
        {
            return await _context.Questions
                .Where(q => q.QuizId == quizId)
                .ToListAsync();
        }

        public async Task<List<Question>> GetRandomQuestionsForQuizAsync(int quizId, int count)
        {
            var allQuestions = await _context.Questions
                .Where(q => q.QuizId == quizId)
                .ToListAsync();

            if (allQuestions.Count == 0)
            {
                return new List<Question>();
            }

            var random = new Random();
            var randomQuestions = allQuestions
                .OrderBy(q => random.Next())
                .Take(count)
                .ToList();

            return randomQuestions;
        }

        public async Task SaveQuizScoreAsync(QuizScore quizScore)
        {
            try
            {
                await _context.QuizScores.AddAsync(quizScore);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error saving quiz score.", ex);
            }
        }

        public async Task<List<QuizScore>> GetQuizScoresForStudentAsync(string studentId)
        {
            try
            {
                return await _context.QuizScores
                .Include(qs => qs.Quiz)
                .Where(qs => qs.StudentId == studentId)
                .ToListAsync();
            }
            catch (Exception ex)
            {
                {
                    throw new InvalidOperationException("Error retrieving quiz scores for student.", ex);
                }
            }
        }

       
    }
}
