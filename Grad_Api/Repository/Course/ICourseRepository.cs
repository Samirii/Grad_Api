using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Repositores;

namespace Grad_Api.Repository
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<CourseReadDto> CreateCourseAsync(CourseCreateDto courseDto);
        Task<List<CourseReadDto>> GetAllCoursesAsync();
        Task<CourseReadDto?> GetCourseAsync(int id);
        Task<List<CourseReadDto>> GetCoursesByCategoryAsync(int categoryId);
        Task<List<CourseReadDto>> GetCoursesByTeacherAsync(string teacherName);
        Task<CourseCategory> GetCourseCategoryAsync(int courseCatId);
        Task<bool> CourseExistsAsync(int id);
        Task<bool> UpdateCourseAsync(int id, CourseUpdateDto course);
        Task<bool> DeleteCourseAsync(int id);
    }
}
