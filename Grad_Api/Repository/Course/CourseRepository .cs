using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStoreAPI.Repositores;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Models.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Grad_Api.Repository.CourseRepository;

namespace Grad_Api.Repository
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        private readonly GradProjDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CourseRepository> _logger;

        public CourseRepository(GradProjDbContext context, IMapper mapper, ILogger<CourseRepository> logger) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> CourseExistsAsync(int id)
        {
            try
            {
                return await _context.Courses.AnyAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if course exists with ID: {Id}", id);
                throw;
            }
        }

        public async Task<CourseReadDto> CreateCourseAsync(CourseCreateDto courseDto)
        {
            try
            {
                var category = await _context.CourseCategories
                    .FirstOrDefaultAsync(c => c.Id == courseDto.CategoryId);

                if (category == null)
                {
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

                return await GetCourseAsync(course.Id) ?? 
                    throw new InvalidOperationException("Failed to retrieve created course");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course with title: {Title}", courseDto.Title);
                throw;
            }
        }

    
        public async Task<List<CourseReadDto>> GetAllCoursesAsync()
        {
            try
            {
                var courses = await _context.Courses
                    .AsNoTracking()
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

                _logger.LogInformation("Retrieved {Count} courses", courses.Count);
                return courses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all courses");
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
                    .Include(c => c.Quizzes)
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

                _logger.LogInformation("Course found: ID={Id}, HasLessons={(course?.Lessons?.Count > 0)}", id);
                return course;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course with ID: {Id}", id);
                throw;
            }
        }

        public async Task<CourseCategory> GetCourseCategoryAsync(int courseCatId)
        {
            try
            {
                return await _context.CourseCategories
                    .FirstOrDefaultAsync(c => c.Id == courseCatId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category with ID: {CategoryId}", courseCatId);
                throw;
            }
        }

        public async Task<List<CourseReadDto>> GetCoursesByCategoryAsync(int categoryId)
        {
            try
            {
                var courses = await _context.Courses
                    .AsNoTracking()
                    .Include(c => c.Category)
                    .Include(c => c.Lessons)
                    .Include(c => c.Quizzes)
                    .Where(c => c.CourseCategoryId == categoryId)
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

                _logger.LogInformation("Retrieved {Count} courses for category {CategoryId}", courses.Count, categoryId);
                return courses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses for category ID: {CategoryId}", categoryId);
                throw;
            }
        }

    

        public async Task<List<CourseReadDto>> GetCoursesByTeacherAsync(string teacherName)
        {
            try
            {
                var courses = await _context.Courses
                    .AsNoTracking()
                    .Include(c => c.Category)
                    .Include(c => c.Lessons)
                    .Include(c => c.Quizzes)
                    .Where(c => c.TeacherName.Equals(teacherName, StringComparison.OrdinalIgnoreCase))
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

                _logger.LogInformation("Retrieved {Count} courses for teacher {TeacherName}", courses.Count, teacherName);
                return courses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses for teacher: {TeacherName}", teacherName);
                throw;
            }
        }


        public async Task<bool> UpdateCourseAsync(int id, CourseUpdateDto course)
        {
            try
            {
                _logger.LogInformation("Attempting to update course with ID: {Id}", id);

                var existingCourse = await _context.Courses.FindAsync(id);
                if (existingCourse == null)
                {
                    _logger.LogWarning("Course not found for update. ID: {Id}", id);
                    return false;
                }

                // Update only the allowed properties
                existingCourse.Title = course.Title;
                existingCourse.Description = course.Description;
                existingCourse.TeacherName = course.TeacherName;
                existingCourse.CourseCategoryId = course.CategoryId;

                
                await UpdateAsync(existingCourse);

                _logger.LogInformation("Course updated successfully. ID: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course with ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete course with ID: {Id}", id);

                var course = await _context.Courses.FindAsync(id);
                if (course == null)
                {
                    _logger.LogWarning("Course not found for deletion. ID: {Id}", id);
                    return false;
                }

                
                await DeleteAsync(id);

                _logger.LogInformation("Course deleted successfully. ID: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course with ID: {Id}", id);
                throw;
            }
        }
        private async Task<bool> CourseExiste(int id)
        {
            try
            {
                _logger.LogInformation("Checking if course exists with ID: {Id}", id);

                // Use the generic repository's Any method to check existence
                var exists = await _context.Courses.AnyAsync(c => c.Id == id);

                _logger.LogInformation("Course existence check result: {Exists} for ID: {Id}", exists, id);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if course exists with ID: {Id}", id);
                throw;
            }
        }

      
    }
}

