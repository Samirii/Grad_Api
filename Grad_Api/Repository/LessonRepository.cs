using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStoreAPI.Repositores;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Models.Lessons;
using Microsoft.EntityFrameworkCore;

namespace Grad_Api.Repository
{
    public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        private readonly GradProjDbContext _context;
        private readonly IMapper _mapper;

        public LessonRepository(GradProjDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ReadLessonDto>> GetLessons()
        {
            var lessons = await _context.Lessons
               .Include(l => l.Course)

               .ToListAsync();

           

            return _mapper.Map<List<ReadLessonDto>>(lessons);
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

        public override async Task<Lesson> AddAsync(Lesson entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(entity.Title))
            {
                throw new InvalidOperationException("Title cannot be empty or whitespace");
            }

            if (string.IsNullOrWhiteSpace(entity.Content))
            {
                throw new InvalidOperationException("Content cannot be empty or whitespace");
            }

            if (entity.CourseId <= 0)
            {
                throw new InvalidOperationException("Invalid CourseId");
            }

            // Verify course exists
            var course = await _context.Courses.FindAsync(entity.CourseId);
            if (course == null)
            {
                throw new InvalidOperationException($"Course with ID {entity.CourseId} does not exist");
            }

            try
            {
                // Ensure strings are trimmed
                entity.Title = entity.Title.Trim();
                entity.Content = entity.Content.Trim();
                if (entity.VideoUrl != null)
                {
                    entity.VideoUrl = entity.VideoUrl.Trim();
                }

                // Add the entity
                await _context.Lessons.AddAsync(entity);

                // Save changes
                await _context.SaveChangesAsync();

                // Reload the entity with its relationships
                var savedLesson = await _context.Lessons
                    .Include(l => l.Course)
                    
                    .FirstOrDefaultAsync(l => l.Id == entity.Id);

                if (savedLesson == null)
                {
                    throw new InvalidOperationException("Failed to retrieve saved lesson");
                }

               
                return savedLesson;
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                throw new DbUpdateException($"Failed to save lesson: {innerMessage}", ex);
            }
        }

        public async Task<bool> CourseExistsAsync(int courseId)
        {
            return await _context.Courses.AnyAsync(c => c.Id == courseId);
        }
    }
}
