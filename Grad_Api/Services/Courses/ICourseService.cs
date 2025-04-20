using Grad_Api.Models.Course;

namespace Grad_Api.Services
{
    public interface ICourseService
    {
        Task<CourseReadDto> CreateCourseAsync(CourseCreateDto courseDto);
        Task<List<CourseReadDto>> GetAllCoursesAsync();
        Task<CourseReadDto?> GetCourseAsync(int id);
        Task<List<CourseReadDto>> GetCoursesByCategoryAsync(int categoryId);
        Task<bool> UpdateCourseAsync(int id, CourseUpdateDto courseDto);
        Task<bool> DeleteCourseAsync(int id);
        Task<bool> Exists(int id);
    }
} 