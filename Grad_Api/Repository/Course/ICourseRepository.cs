using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Repositores;

namespace Grad_Api.Repository
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<Course> CreateCourseAsync(Course course);
        Task<List<CourseReadDto>> GetAllCoursesAsync();
        Task<Course?> GetCourseAsync(int id);
        Task<List<CourseReadDto>> GetCoursesByCategoryAsync(int categoryId);
        Task<List<CourseReadDto>> GetCoursesByTeacherAsync(string teacherName);
        Task<CourseCategory> GetCourseCategoryAsync(int courseCatId);
        Task<bool> CourseExistsAsync(int id);
        Task<bool> UpdateCourseAsync(int id, CourseUpdateDto updateDto);
        Task<bool> DeleteCourseAsync(int id);
        Task<CourseCategory?> GetCategoryByIdAsync(int id);
    }
}
