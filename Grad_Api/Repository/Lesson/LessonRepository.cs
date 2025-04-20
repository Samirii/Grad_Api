using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStoreAPI.Repositores;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Models.Lessons;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Grad_Api.Repository
{
    public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        private readonly GradProjDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<LessonRepository> _logger;

        public LessonRepository(GradProjDbContext context, IMapper mapper, ILogger<LessonRepository> logger) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;   
        }

        public async Task<List<ReadLessonDto>> GetLessons()
        {
            try
            {
                var lessons = await _context.Lessons.Include(l=>l.Course).ToListAsync();
                return _mapper.Map<List<ReadLessonDto>>(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if course exists with ");
                throw;
            }
            
            
        }

        public async Task<ReadLessonDto?> GetLessonsById(int id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
              
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
                return null;

           
            return _mapper.Map<ReadLessonDto>(lesson);
        }

        public async Task<ReadLessonDto> CreateLessonWithQuizCheckAsync(CreateLessonDto dto)
        {
            var lesson = new Lesson
            {
                Title = dto.Title,
                VideoUrl = dto.VideoUrl,
                Content = dto.Content,
                CourseId = dto.CourseId
            };

            // Save lesson first
            await AddAsync(lesson);

            var savedLesson = await _context.Lessons
         
            .FirstOrDefaultAsync(l => l.Id == lesson.Id);

            var result = _mapper.Map<ReadLessonDto>(savedLesson);
            

            return result;
        }

      


        public async Task<List<ReadLessonDto>> GetLessonsByCourseIdAsync(int courseId)
        {
            return await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .Include(l => l.Course)  // Include course if needed
                .ProjectTo<ReadLessonDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<bool> CourseExistsAsync(int courseId)
        {
            return await _context.Courses.AnyAsync(c => c.Id == courseId);
        }

        public async Task<bool> DeleteLessonAsync(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete lesson with ID: {Id}", id);

                var lesson = await _context.Lessons.FindAsync(id);
                if (lesson == null)
                {
                    _logger.LogWarning("lesson not found for deletion. ID: {Id}", id);
                    return false ;
                }


                await DeleteAsync(id);

                _logger.LogInformation("lesson deleted successfully. ID: {Id}", id);
                return true ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Lesson with ID: {Id}", id);
                throw;
            }

        }
    }
}
