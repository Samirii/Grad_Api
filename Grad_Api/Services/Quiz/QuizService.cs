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
using Microsoft.Extensions.Logging;

namespace Grad_Api.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ILogger<QuizService> _logger;

        public QuizService(IQuizRepository quizRepository, ILogger<QuizService> logger)
        {
            _quizRepository = quizRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Grad_Api.Data.Question>> GetQuestionsByQuizIdAsync(int quizId)
        {
            try
            {
                _logger.LogInformation("Retrieving questions for quiz ID: {QuizId}", quizId);
                var questions = await _quizRepository.GetQuestionsByQuizIdAsync(quizId);
                _logger.LogInformation("Successfully retrieved {Count} questions for quiz ID: {QuizId}", questions.Count(), quizId);
                return questions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions for quiz ID: {QuizId}", quizId);
                throw;
            }
        }

        //public async Task<Quiz> GetQuizWithQuestionsByLessonIdAsync(int lessonId)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Retrieving quiz with questions for lesson ID: {LessonId}", lessonId);
        //        var quiz = await _quizRepository.GetQuizWithQuestionsByLessonIdAsync(lessonId);
        //        _logger.LogInformation("Successfully retrieved quiz with {Count} questions for lesson ID: {LessonId}", 
        //            quiz?.Questions?.Count ?? 0, lessonId);
        //        return quiz;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error retrieving quiz with questions for lesson ID: {LessonId}", lessonId);
        //        throw;
        //    }
        //}

        public async Task ImportQuestionsFromExcelAsync(string filePath)
        {
            try
            {
                _logger.LogInformation("Starting import of questions from Excel file: {FilePath}", filePath);
                var excelQuestions = ReadQuestionsFromExcel(filePath);
                _logger.LogInformation("Successfully read {Count} questions from Excel file", excelQuestions.Count);

                var groupedQuestions = excelQuestions
                    .GroupBy(q => q.QuizId)
                    .ToList();

                foreach (var group in groupedQuestions)
                {
                    var quizId = group.Key;
                    _logger.LogInformation("Processing questions for quiz ID: {QuizId}", quizId);

                    var existingQuiz = await _quizRepository.GetQuizByQuizIdAsync(quizId);

                    Quiz quiz;
                    if (existingQuiz == null)
                    {
                        _logger.LogInformation("Creating new quiz with ID: {QuizId}", quizId);
                        quiz = new Quiz
                        {
                            
                            Title = $"Quiz for QuizId {quizId}"
                        };
                        quiz = await _quizRepository.AddQuizAsync(quiz);
                        _logger.LogInformation("Successfully created new quiz with ID: {QuizId}", quizId);
                    }
                    else
                    {
                        _logger.LogInformation("Using existing quiz with ID: {QuizId}", quizId);
                        quiz = existingQuiz;
                    }

                    var questions = group.Select(eq => new Grad_Api.Data.Question
                    {
                        QuestionText = eq.QuestionTitle,
                        OptionA = eq.OptionA,
                        OptionB = eq.OptionB,
                        OptionC = eq.OptionC,
                        OptionD = eq.OptionD,
                        CorrectAnswer = eq.CorrectAnswer,
                        Defficulty = eq.Defficulty,
                        Subject = eq.Subject,
                        CourseCategory = eq.CourseCategory,
                        UnitNumber = eq.UnitNumber,
                        QuizId = quiz.Id,

                    }).ToList();

                    _logger.LogInformation("Adding {Count} questions to quiz ID: {QuizId}", questions.Count, quizId);
                    await _quizRepository.AddQuestionsAsync(questions);
                    _logger.LogInformation("Successfully added questions to quiz ID: {QuizId}", quizId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing questions from Excel file: {FilePath}", filePath);
                throw;
            }
        }

        private List<ExcelQuestion> ReadQuestionsFromExcel(string filePath)
        {
            try
            {
                _logger.LogInformation("Reading questions from Excel file: {FilePath}", filePath);
                var questions = new List<ExcelQuestion>();
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        if (!int.TryParse(row.Cell(11).GetString(), out int quizId))
                        {
                            throw new InvalidOperationException($"Invalid QuizId in row {row.RowNumber()}. Value: '{row.Cell(11).GetString()}'");
                        }
                        questions.Add(new ExcelQuestion
                        {

                            QuizId = quizId,
                            QuestionTitle = row.Cell(1).GetString()?.Trim(),
                            OptionA = row.Cell(2).GetString()?.Trim(),
                            OptionB = row.Cell(3).GetString()?.Trim(),
                            OptionC = row.Cell(4).GetString()?.Trim(),
                            OptionD = row.Cell(5).GetString()?.Trim(),
                            CorrectAnswer = row.Cell(6).GetString()?.Trim(),
                            Defficulty = row.Cell(7).GetString()?.Trim(),
                            Subject = row.Cell(8).GetString()?.Trim(),
                            CourseCategory = row.Cell(9).GetString()?.Trim(),
                            UnitNumber = int.Parse(row.Cell(10).GetString())
                        });
                    }
                }
                _logger.LogInformation("Successfully read {Count} questions from Excel file", questions.Count);
                return questions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading questions from Excel file: {FilePath}", filePath);
                throw;
            }
        }

        public async Task<List<Grad_Api.Data.Question>> GetRandomQuestionsForQuizAsync(int quizId, int count)
        {
            try
            {
                _logger.LogInformation("Retrieving {Count} random questions for quiz ID: {QuizId}", count, quizId);
                var randomQuestions = await _quizRepository.GetRandomQuestionsForQuizAsync(quizId, count);
                _logger.LogInformation("Successfully retrieved {Count} random questions for quiz ID: {QuizId}", 
                    randomQuestions.Count, quizId);
                return randomQuestions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving random questions for quiz ID: {QuizId}", quizId);
                throw new InvalidOperationException("Error retrieving random questions.", ex);
            }
        }

        public async Task<ScoreResponseDto> CalculateAndSaveScoreAsync(string studentId, SubmitAnswersDto submitAnswersDto)
        {
            try
            {
                _logger.LogInformation("Calculating score for student ID: {StudentId}, quiz ID: {QuizId}", 
                    studentId, submitAnswersDto.QuizId);

                var questions = await _quizRepository.GetQuestionsByQuizIdAsync(submitAnswersDto.QuizId);

                if (questions == null || !questions.Any())
                {
                    _logger.LogWarning("No questions found for quiz ID: {QuizId}", submitAnswersDto.QuizId);
                    throw new InvalidOperationException("No questions found for the specified quiz.");
                }

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

                int score = (int)((double)correctAnswers / totalQuestions * 100);
                _logger.LogInformation("Score calculated: {Score}% for student ID: {StudentId}, quiz ID: {QuizId}", 
                    score, studentId, submitAnswersDto.QuizId);

                var quizScore = new QuizScore
                {
                    StudentId = studentId,
                    QuizId = submitAnswersDto.QuizId,
                    Score = score
                };

                await _quizRepository.SaveQuizScoreAsync(quizScore);
                _logger.LogInformation("Score saved successfully for student ID: {StudentId}, quiz ID: {QuizId}", 
                    studentId, submitAnswersDto.QuizId);

                return new ScoreResponseDto
                {
                    TotalQuestions = totalQuestions,
                    CorrectAnswers = correctAnswers,
                    Score = score
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating and saving score for student ID: {StudentId}, quiz ID: {QuizId}", 
                    studentId, submitAnswersDto.QuizId);
                throw new InvalidOperationException("Error calculating and saving score.", ex);
            }
        }

        public async Task<List<QuizScoreDto>> GetQuizScoresForStudentAsync(string studentId)
        {
            try
            {
                _logger.LogInformation("Retrieving quiz scores for student ID: {StudentId}", studentId);
                var quizScores = await _quizRepository.GetQuizScoresForStudentAsync(studentId);
                _logger.LogInformation("Successfully retrieved {Count} quiz scores for student ID: {StudentId}", 
                    quizScores.Count, studentId);

                return quizScores.Select(qs => new QuizScoreDto
                {
                    QuizId = qs.QuizId,
                    StudentId = studentId,
                    Score = qs.Score,
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quiz scores for student ID: {StudentId}", studentId);
                throw new InvalidOperationException("Error retrieving quiz scores for student.", ex);
            }
        }

        public async Task<QuizScoreDto> SendScoreAsync(string studentId, int quizId, int score)
        {
            try
            {
                _logger.LogInformation("Saving quiz score for student ID: {StudentId}, quiz ID: {QuizId}, score: {Score}", 
                    studentId, quizId, score);

                var quizScore = new QuizScore
                {
                    StudentId = studentId,
                    QuizId = quizId,
                    Score = score
                };

                await _quizRepository.SaveQuizScoreAsync(quizScore);
                _logger.LogInformation("Successfully saved quiz score for student ID: {StudentId}, quiz ID: {QuizId}", 
                    studentId, quizId);

                return new QuizScoreDto
                {
                    StudentId = studentId,
                    QuizId = quizId,
                    Score = score
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving quiz score for student ID: {StudentId}, quiz ID: {QuizId}", 
                    studentId, quizId);
                throw new InvalidOperationException("Error saving quiz score", ex);
            }
        }

      

        public Task<Quiz> GetQuizWithQuestionsByLessonIdAsync(int lessonId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Question>> GetQuestionsByUnitAndCategoryAsync(int unitNumber, string courseName, string categoryName)
        {
            _logger.LogInformation("Retrieving Questions  for Unit Number: {unitNumber}");
            var questions = await _quizRepository.GetQuestionsByUnitAndCategoryAsync(unitNumber, courseName, categoryName);
            _logger.LogInformation("Successfully retrieved  random questions for quiz ID: {UnitNumber}",
                  unitNumber);
            return questions;

        }
    }
}