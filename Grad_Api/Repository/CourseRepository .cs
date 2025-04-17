using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStoreAPI.Repositores;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Models.Lessons;
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
                TeacherName = courseDto.TeacherName,
                CourseCategoryId = category.Id, 
                
            };

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            return await GetCourseAsync(course.Id);
        }

        public async Task<List<CourseReadDto>> GetAllCoursesAsync()
        {
            try
            {
                var courses = await _context.Courses
                    .AsNoTracking()
                    .Include(c => c.Category)
                    .Include(c => c.Lessons)
                    .Select(c => new CourseReadDto
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Description = c.Description,
                        TeacherName = c.TeacherName,
                        CategoryName = c.Category != null ? c.Category.Name.Trim() : null,
                        LessonCount = c.Lessons.Count,
                        QuizCount = c.Quizzes.Count,
                        Lessons = c.Lessons.Select(l => new ReadLessonDto
                        {
                            Id = l.Id,
                            Title = l.Title,
                            Content = l.Content,
                            VideoUrl = l.VideoUrl,
                            CourseId = l.CourseId
                        }).ToList()
                    })
                    .ToListAsync();

                Console.WriteLine($"Found {courses.Count} courses");
                return courses;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllCoursesAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<CourseReadDto?> GetCourseAsync(int id)
        {
            try
            {
                var course = await _context.Courses
                    .AsNoTracking()
                    .Include(c => c.Category)
                    .Include(c => c.Lessons)
                    .Where(c => c.Id == id)
                    .Select(c => new CourseReadDto
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Description = c.Description,
                        TeacherName = c.TeacherName,
                        CategoryName = c.Category != null ? c.Category.Name.Trim() : null,
                        LessonCount = c.Lessons.Count,
                        QuizCount = c.Quizzes.Count,
                        Lessons = c.Lessons.Select(l => new ReadLessonDto
                        {
                            Id = l.Id,
                            Title = l.Title,
                            Content = l.Content,
                            VideoUrl = l.VideoUrl,
                            CourseId = l.CourseId
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                Console.WriteLine($"Course found: ID={id}, HasLessons={(course?.Lessons?.Count > 0)}");
                return course;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCourseAsync: {ex.Message}");
                throw;
            }
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
                .Where(c => c.CourseCategoryId == categoryId)
                .Include(c => c.Category)
                .Include(c => c.Lessons)
                .Include(c => c.Quizzes)
                .Select(c => new CourseReadDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    TeacherName = c.TeacherName,
                    CategoryName = c.Category != null ? c.Category.Name.Trim() : null,
                    LessonCount = c.Lessons.Count,
                    QuizCount = c.Quizzes.Count
                })
                .ToListAsync();
        }

        public Task<List<CourseReadDto>> GetCoursesByTeacherAsync(string teacherName)
        {
            throw new NotImplementedException();
        }

        //public async Task<List<CourseReadDto>> GetCoursesByTeacherAsync(string teacherName)
        //{
        //    return await _context.Courses
        //        .Where(c => c.TeacherName == teacherName)
        //        .Include(c => c.Category)
        //        .Include(c => c.Lessons)
        //        .Include(c => c.Quizzes)
        //        .Select(c => new CourseReadDto
        //        {
        //            Id = c.Id,
        //            Title = c.Title,
        //            Description = c.Description,
        //            TeacherName = c.TeacherName,
        //            CategoryName = c.Category != null ? c.Category.Name.Trim() : null,
        //            LessonCount = c.Lessons.Count,
        //            QuizCount = c.Quizzes.Count
        //        })
        //        .ToListAsync();
        //}
    }
}

