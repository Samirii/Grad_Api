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
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;

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

  

        public async Task<Quiz> GetQuizWithQuestionsByLessonIdAsync(int lessonId)
        {
            try
            {
                _logger.LogInformation("Retrieving quiz with questions for lesson ID: {LessonId}", lessonId);
                var quiz = await _quizRepository.GetQuizWithQuestionsByLessonIdAsync(lessonId);
                _logger.LogInformation("Successfully retrieved quiz with {Count} questions for lesson ID: {LessonId}",
                    quiz?.Questions?.Count ?? 0, lessonId);
                return quiz;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quiz with questions for lesson ID: {LessonId}", lessonId);
                throw;
            }
        }

        public async Task ImportQuestionsFromExcelAsync(string filePath)
        {
            try
            {
                _logger.LogInformation("Starting import of questions from Excel file: {FilePath}", filePath);

                // Read questions from the Excel file
                var excelQuestions = ReadQuestionsFromExcel(filePath);
                _logger.LogInformation("Successfully read {Count} questions from Excel file", excelQuestions.Count);

                var groupedQuestions = excelQuestions
                    .GroupBy(q => q.LessonId)
                    .ToList();

                foreach (var group in groupedQuestions)
                {
                    var lessonId = group.Key;

                    // Check if a Quiz already exists for this LessonId
                    var existingQuiz = await _quizRepository.GetQuizByQuizIdAsync(lessonId);

                    Quiz quiz;
                    if (existingQuiz == null)
                    {
                        // Create a new Quiz
                        quiz = new Quiz
                        {
                            Title = $"Quiz for LessonId {lessonId}",
                            LessonId = lessonId
                        };

                        try
                        {
                            // Add the Quiz to the database
                            quiz = await _quizRepository.AddQuizAsync(quiz);
                            _logger.LogInformation("Successfully created new quiz with LessonId: {LessonId}", lessonId);
                        }
                        catch (InvalidOperationException ex)
                        {
                            _logger.LogError(ex, "Skipping quiz creation for invalid LessonId: {LessonId}", lessonId);
                            continue; // Skip invalid LessonId
                        }
                    }
                    else
                    {
                        // Use the existing Quiz
                        quiz = existingQuiz;
                        _logger.LogInformation("Using existing quiz with LessonId: {LessonId}", lessonId);
                    }

                    // Map Excel questions to database questions
                    var questions = group.Select(eq => new Question
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
                        QuizId = quiz.Id
                    }).ToList();

                    // Add the questions to the database
                    await _quizRepository.AddQuestionsAsync(questions);
                    _logger.LogInformation("Successfully added {Count} questions to quiz with LessonId: {LessonId}", questions.Count, lessonId);
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

                    foreach (var row in worksheet.RowsUsed().Skip(1)) // Skip header row
                    {
                        // Parse LessonId
                        if (!int.TryParse(row.Cell(11).GetString(), out int lessonId) || lessonId <= 0)
                        {
                            _logger.LogWarning("Invalid LessonId in row {RowNumber}. Value: '{CellValue}'. Skipping this row.", row.RowNumber(), row.Cell(11).GetString());
                            continue; // Skip invalid rows
                        }

                        // Validate other required fields
                        var questionTitle = row.Cell(1).GetString()?.Trim();
                        if (string.IsNullOrWhiteSpace(questionTitle))
                        {
                            _logger.LogWarning("Missing or empty QuestionTitle in row {RowNumber}. Skipping this row.", row.RowNumber());
                            continue;
                        }

                        // Add valid rows to the list
                        questions.Add(new ExcelQuestion
                        {
                            LessonId = lessonId,
                            QuestionTitle = questionTitle,
                            OptionA = row.Cell(2).GetString()?.Trim(),
                            OptionB = row.Cell(3).GetString()?.Trim(),
                            OptionC = row.Cell(4).GetString()?.Trim(),
                            OptionD = row.Cell(5).GetString()?.Trim(),
                            CorrectAnswer = row.Cell(6).GetString()?.Trim(),
                            Defficulty = row.Cell(7).GetString()?.Trim(),
                            Subject = row.Cell(8).GetString()?.Trim(),
                            CourseCategory = row.Cell(9).GetString()?.Trim(),
                            UnitNumber = int.TryParse(row.Cell(10).GetString(), out int unitNumber) ? unitNumber : 0 // Default to 0 if invalid
                        });
                    }
                }

                _logger.LogInformation("Successfully read {Count} valid questions from Excel file", questions.Count);
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

        //public async Task<ScoreResponseDto> CalculateAndSaveScoreAsync(string studentId, SubmitAnswersDto submitAnswersDto)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Calculating score for student ID: {StudentId}, quiz ID: {QuizId}", 
        //            studentId, submitAnswersDto.QuizId);

        //        var questions = await _quizRepository.GetQuestionsByQuizIdAsync(submitAnswersDto.QuizId);

        //        if (questions == null || !questions.Any())
        //        {
        //            _logger.LogWarning("No questions found for quiz ID: {QuizId}", submitAnswersDto.QuizId);
        //            throw new InvalidOperationException("No questions found for the specified quiz.");
        //        }

        //        int totalQuestions = questions.Count();
        //        int correctAnswers = 0;

        //        foreach (var submittedAnswer in submitAnswersDto.Answers)
        //        {
        //            var question = questions.FirstOrDefault(q => q.Id == submittedAnswer.QuestionId);
        //            if (question != null && question.CorrectAnswer.Equals(submittedAnswer.SelectedAnswer, StringComparison.OrdinalIgnoreCase))
        //            {
        //                correctAnswers++;
        //            }
        //        }

        //        int score = (int)((double)correctAnswers / totalQuestions * 100);
        //        _logger.LogInformation("Score calculated: {Score}% for student ID: {StudentId}, quiz ID: {QuizId}", 
        //            score, studentId, submitAnswersDto.QuizId);

        //        var quizScore = new QuizScore
        //        {
        //            StudentId = studentId,
        //            LessonId = submitAnswersDto.less,
        //            Score = score
        //        };

        //        await _quizRepository.SaveQuizScoreAsync(quizScore);
        //        _logger.LogInformation("Score saved successfully for student ID: {StudentId}, quiz ID: {QuizId}", 
        //            studentId, submitAnswersDto.QuizId);

        //        return new ScoreResponseDto
        //        {
        //            TotalQuestions = totalQuestions,
        //            CorrectAnswers = correctAnswers,
        //            Score = score
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error calculating and saving score for student ID: {StudentId}, quiz ID: {QuizId}", 
        //            studentId, submitAnswersDto.QuizId);
        //        throw new InvalidOperationException("Error calculating and saving score.", ex);
        //    }
        //}

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

        public async Task<QuizScoreDto> SendScoreAsync(QuizScoreDto quizScoreDto)
        {
            try
            {
                _logger.LogInformation("Saving quiz score for student ID: {StudentId}, quiz ID: {QuizId}, score: {Score}",
                    quizScoreDto);
                var existingScore = await _quizRepository.GetQuizScoreAsync(quizScoreDto.StudentId, quizScoreDto.LessonId);
                if (existingScore != null)
                {
                    _logger.LogWarning("Student {StudentId} has already submitted a quiz for lesson {LessonId}",
                        quizScoreDto.StudentId, quizScoreDto.LessonId);
                    throw new InvalidOperationException("You cannot submit the quiz for this lesson again.");
                }
                var quizScore = new QuizScore
                {
                    StudentId = quizScoreDto.StudentId,
                    LessonId = quizScoreDto.LessonId,
                    Score = quizScoreDto.Score,
                };

                await _quizRepository.SaveQuizScoreAsync(quizScore);
                _logger.LogInformation("Successfully saved quiz score for student ID: {StudentId}, quiz ID: {QuizId}",
                    quizScoreDto);

                return new QuizScoreDto
                {
                     StudentId = quizScoreDto.StudentId,
                    LessonId = quizScoreDto.LessonId,
                    Score = quizScoreDto.Score * 10
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving quiz score for student ID: {StudentId}, quiz ID: {QuizId}",
                   quizScoreDto);
                throw new InvalidOperationException("Error saving quiz score", ex);
            }
        }



      

       
        public async Task<List<Question>> GetQuestionsByUnitAndCategoryAsync(int unitNumber, string courseName, string categoryName)
        {
            _logger.LogInformation("Retrieving Questions  for Unit Number: {unitNumber}");
            var questions = await _quizRepository.GetQuestionsByUnitAndCategoryAsync(unitNumber, courseName, categoryName);
            _logger.LogInformation("Successfully retrieved  random questions for quiz ID: {UnitNumber}",
                  unitNumber);
            return questions;

        }

        public Task<ScoreResponseDto> CalculateAndSaveScoreAsync(string studentId, SubmitAnswersDto submitAnswersDto)
        {
            throw new NotImplementedException();
        }

        public async Task<List<QuizScoreDto>> GetAllQuizScoresAsync()
        {
            var quizScore =  await _quizRepository.GetAllQuizScores();
           return quizScore.Select(q => new QuizScoreDto
            {
                StudentId = q.StudentId,
                LessonId = q.LessonId,
                Score = q.Score
            }).ToList();
        }

    }
    }
