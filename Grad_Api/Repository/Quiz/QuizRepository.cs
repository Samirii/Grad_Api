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
                // Validate that the LessonId exists in the Lesson table
                var lessonExists = await _context.Lessons.AnyAsync(l => l.Id == quiz.LessonId);
                if (!lessonExists)
                {
                    throw new InvalidOperationException($"LessonId {quiz.LessonId} does not exist in the Lesson table. Cannot create a Quiz without a valid Lesson.");
                }

                // Add the quiz to the database
                await _context.Quizzes.AddAsync(quiz);
                await _context.SaveChangesAsync();

                return quiz;
            }
            catch (Exception ex)
            {

                
                throw new InvalidOperationException("Error adding quiz. Please ensure the LessonId exists in the database.", ex);
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

        public async Task<Quiz> GetQuizWithQuestionsByLessonIdAsync(int lessonId)
        {
            var quizes = await _context.Quizzes.
                Include(q=>q.Questions)
                .FirstOrDefaultAsync(q=>q.LessonId==lessonId);
            return quizes; 
        }

        public async Task<QuizScore?> GetQuizScoreAsync(string studentId, int lessonId)
        {
            return await _context.QuizScores.FirstOrDefaultAsync(q=>q.StudentId==studentId && q.LessonId == lessonId);    
        }

        public async Task<List<QuizScore>> GetAllQuizScores()
        {
            return await _context.QuizScores.Include(q=>q.Student)
                .Include(q=>q.Lesson)
                .ToListAsync();
        }
    }
    
}
