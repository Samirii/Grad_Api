using Grad_Api.Models.PythonAi;
using Grad_Api.Repository;
using Grad_Api.Repository.Performance;
using System.Text;
using System.Text.Json;

namespace Grad_Api.Services.Performance
{
    public class StudentPerformanceService : IStudentPerformanceService
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<StudentPerformanceService> _logger;

        private readonly IPerformanceRepository _repo;


        public StudentPerformanceService(IPerformanceRepository repo, HttpClient httpClient, ILogger<StudentPerformanceService> logger)
        {
            _repo = repo;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<object?> GetPredictedPerformanceAsync(string studentId, int courseId)
        {
            var quizScores = await _repo.GetQuizScoresForStudentInCourseAsync(studentId, courseId);
            var course = await _repo.GetCourseByIdAsync(courseId);

            if (!quizScores.Any())
            {
                _logger.LogWarning("No quiz scores found for student {StudentId} in course {CourseId}", studentId, courseId);
                return null;
            }

            var averageScore = quizScores.Average(q => q.Score);
            var numQuizzes = quizScores.Count;
            var lastQuizScore = quizScores.OrderBy(q => q.Id).LastOrDefault()?.Score ?? 0;

            if (double.IsNaN(averageScore) || numQuizzes == 0)
            {
                throw new InvalidOperationException("Invalid quiz score data");
            }

            const double avgTimeSpent = 30.0;

            var input = new FlaskPerformanceInput
            {
                AvgPreviousScore = averageScore,
                NumQuizzesTaken = numQuizzes,
                LastScore = lastQuizScore,
               

            };
            var json = JsonSerializer.Serialize(input);
            _logger.LogInformation("Sending to Flask API: {Json}", json);

            try
            {
                var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/predict", input);

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Flask API responded with status {StatusCode}: {Content}",
                    response.StatusCode, responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch prediction from Flask API. Status: {response.StatusCode}, Response: {responseContent}");
                }

                var prediction = JsonSerializer.Deserialize<PredictionResponse>(responseContent);

                return new
                {
                    CourseName = course?.Title,
                    PredictedPerformance = prediction?.PredictedPerformance,
                    Details = input
                    
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling Flask API for student {StudentId} in course {CourseId}", studentId, courseId);
                throw;
            }

        }
    }
}




        //    public StudentPerformanceService(HttpClient httpClient, IQuizRepository quizRepository)
        //    {
        //        _httpClient = httpClient;
        //        _quizRepository = quizRepository;
        //        _quizRepository = quizRepository;
        //    }
        //    public async Task<PredictionResponse> PredictStudentPerformanceAsync(PredictionRequest request)
        //    {
        //        var apiUrl = "http://localhost:5000/predict";
        //        var jsonPayload = JsonSerializer.Serialize(request);
        //        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        //        try
        //        {

        //            var response = await _httpClient.PostAsync(apiUrl, content);
        //            response.EnsureSuccessStatusCode();
        //            var responseBody = await response.Content.ReadAsStringAsync();
        //            var predictionResult = JsonSerializer.Deserialize<PredictionResponse>(responseBody);
        //            return predictionResult;
        //        }
        //        catch (Exception ex)
        //        {
        //            // Log the error and rethrow
        //            throw new Exception($"Error calling Python API: {ex.Message}", ex);
        //        }
        //    }
        //    public async Task<StudentPerformanceMetrics> GetStudentPerformanceMetricsAsync(string studentId)
        //    {
        //        try
        //        {
        //            // Retrieve all quiz scores for the student using the repository
        //            var quizScores = await _quizRepository.GetQuizScoresForStudentAsync(studentId);
        //            if (quizScores == null || !quizScores.Any())
        //            {
        //                throw new InvalidOperationException("No quiz scores found for the specified student.");
        //            }

        //            int numQuizzesTaken = quizScores.Count;


        //            double totalScore = quizScores.Sum(qs => qs.Score);
        //            double avgScore = totalScore / numQuizzesTaken;

        //            return new StudentPerformanceMetrics
        //            {

        //                StudentId = studentId,
        //                NumQuizzesTaken = numQuizzesTaken,

        //                 AvgScore = Math.Round(avgScore, 2)
        //            };

        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception($"Error retrieving performance metrics for student ID: {studentId}. Error: {ex.Message}", ex);
        //        }
        //    }
        //}
    

