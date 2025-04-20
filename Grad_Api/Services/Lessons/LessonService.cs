using Grad_Api.Data;
using Grad_Api.Models.Lessons;
using Grad_Api.Repository;

namespace Grad_Api.Services.Lesson
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository; 
        public LessonService(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
            
        }

        public async Task<bool> CourseExistsAsync(int courseId)
        {
            return await _lessonRepository.CourseExistsAsync(courseId);
        }

        public async Task<ReadLessonDto> CreateLessonAsync(CreateLessonDto dto)
        {
            var createdLesson = await _lessonRepository.CreateLessonWithQuizCheckAsync(dto);
            return createdLesson;
        }

        public async Task<bool> DeleteLessonAsync(int id)
        {
            var existinglesson = await _lessonRepository.GetLessonsById(id);
            var result = await _lessonRepository.DeleteLessonAsync(id);
            return true;
        }

        public async Task<List<ReadLessonDto>> GetAllLessonsAsync()
        {
           var lessons = await _lessonRepository.GetLessons();
            var lessonDto = lessons.Select(lessons => new ReadLessonDto
            {
                Id = lessons.Id,
                Title = lessons.Title,
                Content = lessons.Content,
                CourseId = lessons.CourseId,
                VideoUrl = lessons.VideoUrl,
                HasQuiz = lessons.HasQuiz,
                
            }).ToList();
            return lessonDto;
            
            
        }

        public async Task<ReadLessonDto> GetLessonAsync(int id)
        {
            var lesson = await _lessonRepository.GetLessonsById(id);
            if (lesson == null)
                return null;
            var lessonDto = new ReadLessonDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Content = lesson.Content,
                CourseId = lesson.CourseId,
                VideoUrl = lesson.VideoUrl,
                HasQuiz = lesson.HasQuiz
            };

            return lessonDto;

        }

        public async Task<List<ReadLessonDto>> GetLessonsByCourseIdAsync(int courseId)
        {
            return await _lessonRepository.GetLessonsByCourseIdAsync(courseId);
        }
    }
}
