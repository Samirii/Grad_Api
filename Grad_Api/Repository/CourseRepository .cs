using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStoreAPI.Repositores;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Microsoft.EntityFrameworkCore;
using static Grad_Api.Repository.CourseRepository;

namespace Grad_Api.Repository
{
 
        public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
            private readonly GradProjDbContext _context;
            private readonly IMapper _mapper;

            public CourseRepository(GradProjDbContext context, IMapper mapper) : base(context)
            {
                _context = context;
                _mapper = mapper;
            }

        public async Task<CourseReadDto> CreateCourseAsync(CourseCreateDto courseDto)
        {
            // Find or create category
            var category = await _context.CourseCategories
                .FirstOrDefaultAsync(c => c.Id == courseDto.CategoryId);

            if (category == null)
            {
                // If category doesn't exist, use default (ID = 1)
                category = await _context.CourseCategories.FindAsync(1);
                if (category == null)
                {
                    throw new InvalidOperationException("Default category not found");
                }
            }

            var course = new Course
            {
                Title = courseDto.Title,
                Description = courseDto.Description,
                ThumbnailUrl = courseDto.ThumbnailUrl,
                TeacherName = courseDto.TeacherName,
                CategoryId = category.Id, // Use resolved category ID
                
            };

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            return await GetCourseAsync(course.Id);
        }

        public async Task<List<CourseReadDto>> GetAllCoursesAsync()
            {
                return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Lessons)
                .Include(c => c.Quizzes)
                .Select(c => new CourseReadDto
                {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                ThumbnailUrl = c.ThumbnailUrl,
                TeacherName = c.TeacherName,
                CategoryName = c.Category.Name,
                LessonCount = c.Lessons.Count,
                QuizCount = c.Quizzes.Count
            })
            .ToListAsync();
        }

            public async Task<CourseReadDto?> GetCourseAsync(int id)
            {
                var course = await _context.Courses
                    .Include(c => c.Category) // Include category
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (course == null)
                    return null;

                return _mapper.Map<CourseReadDto>(course);
            }

        public async Task<CourseCategory> GetCourseCategoryAsync(int courseCatId)
        {
            return await _context.CourseCategories
                   .FirstOrDefaultAsync(c => c.Id == courseCatId);
        }

        // Additional useful methods
        public async Task<List<CourseReadDto>> GetCoursesByCategoryAsync(int categoryId)
            {
                return await _context.Courses
                    .Include(c => c.Category)
                    .Where(c => c.CategoryId == categoryId)
                    .ProjectTo<CourseReadDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }

            public async Task<List<CourseReadDto>> GetCoursesByTeacherAsync(string teacherName)
            {
                return await _context.Courses
                    .Include(c => c.Category)
                    .Where(c => c.TeacherName == teacherName)
                    .ProjectTo<CourseReadDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
        }
    }

