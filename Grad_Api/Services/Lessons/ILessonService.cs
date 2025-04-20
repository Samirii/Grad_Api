using Grad_Api.Models.Course;
using Grad_Api.Models.Lessons;

namespace Grad_Api.Services.Lesson
{
    public interface ILessonService
    {
        Task<List< ReadLessonDto>> GetAllLessonsAsync();
        Task<ReadLessonDto> GetLessonAsync(int id);
        Task<List<ReadLessonDto>> GetLessonsByCourseIdAsync(int courseId);
        Task<ReadLessonDto> CreateLessonAsync(CreateLessonDto dto);
        Task<bool> CourseExistsAsync(int courseId);
        Task<bool> DeleteLessonAsync(int id);
    }
}
