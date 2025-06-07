using Grad_Api.Data;
using Grad_Api.Repositores;
using Microsoft.AspNetCore.Mvc;

namespace Grad_Api.Repository.Performance
{
    public interface IPerformanceRepository
    {
        Task<List<QuizScore>> GetQuizScoresForStudentInCourseAsync(string studentId, int courseId);
        Task<Course?> GetCourseByIdAsync(int courseId); 
       

    }

}
