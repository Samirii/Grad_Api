using Grad_Api.Models.PythonAi;

namespace Grad_Api.Services.Performance
{
    public interface IStudentPerformanceService
    {
        Task<object?> GetPredictedPerformanceAsync(string studentId, int courseId);
        Task<object?> GetAllCoursesPerformanceAsync(string studentId);



    }
}
