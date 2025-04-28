using Grad_Api.Data;
using Grad_Api;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Grad_Api.Models.Quiz;
using Grad_Api.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace Grad_Api.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;

        public QuizService(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }

        public async Task<IEnumerable<Grad_Api.Data.Question>> GetQuestionsByQuizIdAsync(int quizId)
        {
            return await _quizRepository.GetQuestionsByQuizIdAsync(quizId);
        }

        public async Task<Quiz> GetQuizWithQuestionsByLessonIdAsync(int lessonId)
        {
            return await _quizRepository.GetQuizWithQuestionsByLessonIdAsync(lessonId);
        }

        public async Task ImportQuestionsFromExcelAsync(string filePath)
        {
            var excelQuestions = ReadQuestionsFromExcel(filePath);

            
            var groupedQuestions = excelQuestions
                .GroupBy(q => q.LessonId)
                .ToList();

            foreach (var group in groupedQuestions)
            {
                var lessonId = group.Key;
                
                // Check if a quiz already exists for this lesson
                var existingQuiz = await _quizRepository.GetQuizByLessonIdAsync(lessonId);

                Quiz quiz;
                if (existingQuiz == null)
                {
                    // Create new quiz if one doesn't exist
                    quiz = new Quiz
                    {
                        LessonId = lessonId,
                        Title = $"Quiz for Lesson {lessonId}"
                    };
                    quiz = await _quizRepository.AddQuizAsync(quiz);
                }
                else
                {
                    quiz = existingQuiz;
                }

                // Map Excel data to Question entities
                var questions = group.Select(eq => new Grad_Api.Data.Question
                {
                    QuizId = quiz.Id,
                    QuestionText = eq.QuestionTitle,
                    OptionA = eq.OptionA,
                    OptionB = eq.OptionB,
                    OptionC = eq.OptionC,
                    OptionD = eq.OptionD,
                    CorrectAnswer = eq.CorrectAnswer
                }).ToList();

                await _quizRepository.AddQuestionsAsync(questions);
            }
        }

        private List<ExcelQuestion> ReadQuestionsFromExcel(string filePath)
        {
            var questions = new List<ExcelQuestion>();
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                foreach (var row in worksheet.RowsUsed().Skip(1)) // Skip header row
                {
                    questions.Add(new ExcelQuestion
                    {
                        LessonId = int.Parse(row.Cell(1).GetString()), // Column 1: LessonId
                        QuestionTitle = row.Cell(2).GetString(),      // Column 2: QuestionText
                        OptionA = row.Cell(3).GetString(),            // Column 3: Option1
                        OptionB = row.Cell(4).GetString(),            // Column 4: Option2
                        OptionC = row.Cell(5).GetString(),            // Column 5: Option3
                        OptionD = row.Cell(6).GetString(),            // Column 6: Option4
                        CorrectAnswer = row.Cell(7).GetString()       // Column 7: CorrectAnswer
                    });
                }
            }
            return questions;
        }
        public async Task<List<Grad_Api.Data.Question>> GetRandomQuestionsForQuizAsync(int quizId, int count)
        {
            try
            {
                // Fetch random questions from the repository
                var randomQuestions = await _quizRepository.GetRandomQuestionsForQuizAsync(quizId, count);

                

                return randomQuestions;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving random questions.", ex);
            }
        }

        public async Task<ScoreResponseDto> CalculateAndSaveScoreAsync(string studentId, SubmitAnswersDto submitAnswersDto)
        {
            try
            {
                // Fetch the correct answers for the quiz
                var questions = await _quizRepository.GetQuestionsByQuizIdAsync(submitAnswersDto.QuizId);

                if (questions == null || !questions.Any())
                {
                    throw new InvalidOperationException("No questions found for the specified quiz.");
                }

                // Validate and calculate the score
                int totalQuestions = questions.Count();
                int correctAnswers = 0;

                foreach (var submittedAnswer in submitAnswersDto.Answers)
                {
                    var question = questions.FirstOrDefault(q => q.Id == submittedAnswer.QuestionId);
                    if (question != null && question.CorrectAnswer.Equals(submittedAnswer.SelectedAnswer, StringComparison.OrdinalIgnoreCase))
                    {
                        correctAnswers++;
                    }
                }

                // Calculate the score (percentage)
                int score = (int)((double)correctAnswers / totalQuestions * 100);

                // Save the score to the database
                var quizScore = new QuizScore
                {
                    StudentId = studentId,
                    QuizId = submitAnswersDto.QuizId,
                    Score = score
                };

                await _quizRepository.SaveQuizScoreAsync(quizScore);

                // Return the result
                return new ScoreResponseDto
                {
                    TotalQuestions = totalQuestions,
                    CorrectAnswers = correctAnswers,
                    Score = score
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error calculating and saving score.", ex);
            }
        }

        public async Task<List<QuizScoreDto>> GetQuizScoresForStudentAsync(string studentId)
        {
            try
            {
                var quizScores = await _quizRepository.GetQuizScoresForStudentAsync(studentId);

                // Map to DTOs
                return quizScores.Select(qs => new QuizScoreDto
                {
                    QuizId = qs.QuizId,
                    StudentId=studentId,
                    Score = qs.Score,
                    
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error retrieving quiz scores for student.", ex);
            }
        }
         public async Task<QuizScoreDto> SendScoreAsync(string studentId, int quizId, int score)
        {
            try
            {
                // Create new quiz score
                var quizScore = new QuizScore
                {
                    StudentId = studentId,
                    QuizId = quizId,
                    Score = score
                };

                // Save the score to the database
                await _quizRepository.SaveQuizScoreAsync(quizScore);

                // Return the score DTO
                return new QuizScoreDto
                {
                    StudentId = studentId,
                    QuizId = quizId,
                    Score = score
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error saving quiz score", ex);
            }
        }
    }
}