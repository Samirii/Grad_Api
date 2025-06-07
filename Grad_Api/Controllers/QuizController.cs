using Grad_Api.Models.Quiz;
using Grad_Api.Services;
using Grad_Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Grad_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]



    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ImportQuestions([FromForm] FileUploadRequest request)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                // Validate the file extension
                var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
                if (!new[] { ".xlsx", ".xlsm", ".xltx", ".xltm" }.Contains(fileExtension))
                {
                    return BadRequest("Invalid file format. Only .xlsx, .xlsm, .xltx, and .xltm files are allowed.");
                }

                // Create a temporary file with the correct extension
                var tempDirectory = Path.GetTempPath();
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(tempDirectory, fileName);

                try
                {
                    // Save the uploaded file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.File.CopyToAsync(stream);
                    }

                    // Process the file
                    await _quizService.ImportQuestionsFromExcelAsync(filePath);
                    return Ok("Questions imported successfully.");
                }
                finally
                {
                    // Clean up
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    

        [HttpGet("lesson/{lessonId}")]
        public async Task<IActionResult> GetQuizByLessonId(int lessonId)
        {
            try
            {
                var quiz = await _quizService.GetQuizWithQuestionsByLessonIdAsync(lessonId);
                if (quiz == null)
                {
                    return NotFound($"No quiz found for LessonId {lessonId}");
                }
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the quiz.");
            }
        }

        [HttpGet("{quizId}/random-questions")]
        public async Task<IActionResult> GetRandomQuestionsForQuiz(int quizId, [FromQuery] int count = 20)
        {
            try
            {
                // Validate the count
                if (count <= 0 || count > 100) // Limit to a maximum of 100 questions
                {
                    return BadRequest("Invalid count value. Count must be between 1 and 100.");
                }

                // Fetch random questions
                var randomQuestions = await _quizService.GetRandomQuestionsForQuizAsync(quizId, count);

                // Return the questions
                return Ok(randomQuestions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //[HttpPost("submit-answers")]
        //public async Task<IActionResult> SubmitAnswers([FromBody] SubmitAnswersDto submitAnswersDto, [FromQuery] string studentId)
        //{
        //    try
        //    {
        //        // Validate the input
        //        if (submitAnswersDto == null || submitAnswersDto.Answers == null || !submitAnswersDto.Answers.Any())
        //        {
        //            return BadRequest("Invalid input. Please provide valid answers.");
        //        }

        //        // Calculate and save the score
        //        var scoreResponse = await _quizService.CalculateAndSaveScoreAsync(studentId, submitAnswersDto);

        //        // Return the score
        //        return Ok(scoreResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}
        [HttpGet("scores/{studentId}")]
        public async Task<IActionResult> GetQuizScoresForStudent(string studentId)
        {
            try
            {
                var quizScores = await _quizService.GetQuizScoresForStudentAsync(studentId);
                return Ok(quizScores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("score")]
        public async Task<IActionResult> PostScore([FromBody] QuizScoreDto scoreDto)
        {
            try
            {
                if (scoreDto == null)
                {
                    return BadRequest("Score data is required");
                }

                if (string.IsNullOrEmpty(scoreDto.StudentId))
                {
                    return BadRequest("Student ID is required");
                }


                if (scoreDto.Score < 0 || scoreDto.Score > 100)
                {
                    return BadRequest("Score must be between 0 and 100");
                }

                var result = await _quizService.SendScoreAsync(scoreDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("from-csv")]
        public IActionResult GetQuestionsFromCsv()
        {
            string csvFilePath = "path/to/final_Merged_Questions_Consolidated.csv";

            var questionService = new QuestionService();
            var questions = questionService.ReadQuestionsFromCsv(csvFilePath);

            return Ok(questions);
        }
        [HttpGet("questions")]
        public IActionResult GetQuestionsByFilters(
    [FromQuery] string subjectName,
    [FromQuery] int unitNumber,
    [FromQuery] string prepLevel,
             [FromQuery] int count = 10
      )
            
        {
            try
            {


                // Validation
                if (string.IsNullOrWhiteSpace(subjectName))
                {
                    return BadRequest("Subject name is required.");
                }
                if (unitNumber <= 0)
                {
                    return BadRequest("Unit number must be a positive integer.");
                }
                if (count <= 0 || count > 100) // Limit to a maximum of 100 questions
                {
                    return BadRequest("Invalid count value. Count must be between 1 and 100.");
                }
                var normalizedSubject = subjectName.Replace(" ", "").ToLowerInvariant();

                

                // Read CSV data
                string csvFilePath = "path/to/final_Merged_Questions_Consolidated.csv";
                var questionService = new QuestionService();
                var allQuestions = questionService.ReadQuestionsFromCsv(csvFilePath);

                // Filter questions
                var filteredQuestions = allQuestions
                    .Where(q =>
                        q.Subject.Replace(" ", "").ToLowerInvariant() == normalizedSubject &&
                        q.UnitNumber == unitNumber &&
                        q.CourseCategory.Replace(" ", "").ToLowerInvariant() == prepLevel
                        )
                    .ToList();
                if (filteredQuestions.Count == 0)
                    return NotFound("No questions found matching the criteria.");
                var random = new Random();
                var randomQuestions = filteredQuestions
                .OrderBy(q => random.Next()) 
                .Take(10)                     
                .ToList();

                return Ok(randomQuestions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("all")]
       public async Task<IActionResult> GetAllQuizzesScore()
        {
            var scores = await _quizService.GetAllQuizScoresAsync();
            return Ok(scores);

        }
    }
}
