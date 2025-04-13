using BookStoreAPI.Repositores;
using Grad_Api.Data;
using Grad_Api.Models.Course;

namespace Grad_Api.Repository
{
    public interface ICourseRepository :IGenericRepository<Course> 
    {
        Task<List<CourseReadDto>> GetAllCoursesAsync();
        Task<CourseReadDto?> GetCourseAsync(int id);
        Task<List<CourseReadDto>> GetCoursesByCategoryAsync(int categoryId);
        Task<List<CourseReadDto>> GetCoursesByTeacherAsync(string teacherName);
        Task<CourseReadDto> CreateCourseAsync(CourseCreateDto courseDto);
        Task<CourseCategory> GetCourseCategoryAsync(int courseCatId);

    }
}
