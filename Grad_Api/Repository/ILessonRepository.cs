using BookStoreAPI.Repositores;
using Grad_Api.Data;
using Grad_Api.Models.Lessons;

namespace Grad_Api.Repository
{
    public interface ILessonRepository: IGenericRepository<Lesson>
    {
        Task<List<ReadLessonDto>> GetLessons();
        Task<ReadLessonDto?> GetLessonsById(int id);
        Task<ReadLessonDto> CreateLessonWithQuizCheckAsync(CreateLessonDto dto);
        Task<bool> CourseExistsAsync(int courseId);
        Task<List<ReadLessonDto>> GetLessonsByCourseIdAsync(int courseId);
    }
}
