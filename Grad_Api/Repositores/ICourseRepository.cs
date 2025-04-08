
using Grad_Api.Data;

namespace Grad_Api.Repositores
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<IEnumerable<Course>> GetCoursesByTeacherAsync(string teacherId);
        Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int categoryId);
        Task<bool> IsTeacherOfCourseAsync(int courseId, string teacherId);
    }
}