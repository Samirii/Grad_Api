using Grad_Api.Data;
using Grad_Api.Models;
using Grad_Api.Models.Quiz;
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

      


        public async Task<Quiz> AddQuizAsync(Quiz quiz)
        {
            try
            {
                // Enable IDENTITY_INSERT
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Quiz ON");

                // Add the quiz to the database
                await _context.Quizzes.AddAsync(quiz);
                await _context.SaveChangesAsync();

                // Disable IDENTITY_INSERT
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Quiz OFF");

                return quiz;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error adding quiz.", ex);
            }
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

        public async Task<Quiz> GetQuizByQuizIdAsync(int quizId)
        {
            try
            {
                return await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == quizId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving quiz scores for student.", ex);
            }
        }
        public async Task<List<Question>> GetQuestionsByUnitAndCategoryAsync(int unitNumber, string courseName, string categoryName)
        {
            var questions = await _context.Questions
            .Where(q => q.CourseCategory.Trim().ToLower() == categoryName.Trim().ToLower() &&
                        q.UnitNumber == unitNumber &&
                        q.Subject.Trim().ToLower() == courseName.Trim().ToLower())
            .Select(q => new Question
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                CorrectAnswer = q.CorrectAnswer,
                Defficulty = q.Defficulty,
                Subject = q.Subject,
                CourseCategory = q.CourseCategory,
                UnitNumber = q.UnitNumber,
                
                
            })
            .ToListAsync();

            return questions;
        }
    
}
    
}
