using Grad_Api.Models.PythonAi;
using Grad_Api.Services.Performance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace Grad_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceController : ControllerBase
    {
        private readonly StudentPerformanceService _studentPerformanceService;

        public PerformanceController(StudentPerformanceService studentPerformanceService)
        {
            _studentPerformanceService = studentPerformanceService;
        }
        //[HttpPost("predict")]
        //public async Task<ActionResult<PredictionResponse>> PredictPerformance([FromBody] PredictionRequest request)
        //{
        //    try
        //    {
        //        // Call the Python API to get the prediction
        //        var prediction = await _pythonApiService.PredictStudentPerformanceAsync(request);
        //        return Ok(prediction);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"An error occurred: {ex.Message}");
        //    }
        //}
        //[HttpGet("{studentId}/metrics")]
        //public async Task<ActionResult<StudentPerformanceMetrics>> GetStudentPerformanceMetrics(string studentId)
        //{
        //    try
        //    {
        //        var metrics = await _pythonApiService.GetStudentPerformanceMetricsAsync(studentId);
        //        return Ok(metrics);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"An error occurred: {ex.Message}");
        //    }
        //}


        [HttpGet("{studentId}/{courseId}")]
        public async Task<IActionResult> GetPerformance(string studentId, int courseId)
        {
            var result = await _studentPerformanceService.GetPredictedPerformanceAsync(studentId, courseId);
            if (result == null)
                return NotFound("No quiz data available.");

            return Ok(result);
        }
    }
}