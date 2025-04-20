using AutoMapper;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Models.Lessons;
using Grad_Api.Repository;
using Grad_Api.Services;
using Microsoft.Extensions.Logging;

namespace Grad_Api.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<CourseService> _logger;
        private readonly IMapper _mapper;

        public CourseService(
            ICourseRepository courseRepository, 
            ILogger<CourseService> logger,
            IMapper mapper)
        {
            _courseRepository = courseRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<CourseReadDto> CreateCourseAsync(CourseCreateDto courseDto)
        {
            try
            {
                _logger.LogInformation("Creating new course: {Title}", courseDto.Title);

                // Check if category exists
                var category = await _courseRepository.GetCourseCategoryAsync(courseDto.CategoryId);
                if (category == null)
                {
                    _logger.LogWarning("Category not found with ID: {CategoryId}", courseDto.CategoryId);
                    throw new InvalidOperationException($"Category with ID {courseDto.CategoryId} not found");
                }

                // Map DTO to Course entity
                var course = new Course
                {
                    Title = courseDto.Title,
                    Description = courseDto.Description,
                    TeacherName = courseDto.TeacherName,
                    CourseCategoryId = category.Id
                };

                // Create the course using the repository
                var createdCourse = await _courseRepository.CreateCourseAsync(courseDto);
                _logger.LogInformation("Course created successfully with ID: {Id}", createdCourse.Id);

                // Return the created course details
                return await GetCourseAsync(createdCourse.Id) ?? 
                    throw new InvalidOperationException("Failed to retrieve created course");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course: {Title}", courseDto.Title);
                throw;
            }
        }

        public async Task<List<CourseReadDto>> GetAllCoursesAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all courses");
                var courses = await _courseRepository.GetAllCoursesAsync();
                
                var courseDtos = courses.Select(course => new CourseReadDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    TeacherName = course.TeacherName,
                    CategoryName = course.CategoryName,
                    LessonCount = course.LessonCount,
                   
                    Lessons = course.Lessons?.Select(l => new ReadLessonDto
                    {
                        Id = l.Id,
                        Title = l.Title,
                        Content = l.Content,
                        VideoUrl = l.VideoUrl,
                        CourseId = l.CourseId
                    }).ToList() ?? new List<ReadLessonDto>()
                }).ToList();

                _logger.LogInformation("Retrieved {Count} courses", courseDtos.Count);
                return courseDtos;
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
                _logger.LogInformation("Retrieving course with ID: {Id}", id);
                var course = await _courseRepository.GetCourseAsync(id);
                
                if (course == null)
                {
                    _logger.LogWarning("Course not found with ID: {Id}", id);
                    return null;
                }

                var courseDto = new CourseReadDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    TeacherName = course.TeacherName,
                    CategoryName = course.CategoryName,
                    LessonCount = course.Lessons?.Count ?? 0,
                    
                    Lessons = course.Lessons?.Select(l => new ReadLessonDto
                    {
                        Id = l.Id,
                        Title = l.Title,
                        Content = l.Content,
                        VideoUrl = l.VideoUrl,
                        CourseId = l.CourseId
                    }).ToList() ?? new List<ReadLessonDto>()
                };

                _logger.LogInformation("Course retrieved successfully: {Id}", id);
                return courseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course with ID: {Id}", id);
                throw;
            }
        }

        public async Task<List<CourseReadDto>> GetCoursesByCategoryAsync(int categoryId)
        {
            try
            {
                _logger.LogInformation("Retrieving courses for category ID: {CategoryId}", categoryId);
                var courses = await _courseRepository.GetCoursesByCategoryAsync(categoryId);
                
                var courseDtos = courses.Select(course => new CourseReadDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    TeacherName = course.TeacherName,
                    CategoryName = course.CategoryName,
                    LessonCount = course.Lessons?.Count ?? 0,
                   
                    Lessons = course.Lessons?.Select(l => new ReadLessonDto
                    {
                        Id = l.Id,
                        Title = l.Title,
                        Content = l.Content,
                        VideoUrl = l.VideoUrl,
                        CourseId = l.CourseId
                    }).ToList() ?? new List<ReadLessonDto>()
                }).ToList();

                _logger.LogInformation("Retrieved {Count} courses for category {CategoryId}", courseDtos.Count, categoryId);
                return courseDtos;
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
                _logger.LogInformation("Retrieving courses for teacher: {TeacherName}", teacherName);
                var allCourses = await _courseRepository.GetAllCoursesAsync();
                var teacherCourses = allCourses.Where(c => c.TeacherName.Equals(teacherName, StringComparison.OrdinalIgnoreCase)).ToList();
                
                var courseDtos = teacherCourses.Select(course => new CourseReadDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    TeacherName = course.TeacherName,
                    CategoryName = course.CategoryName,
                    LessonCount = course.Lessons?.Count ?? 0,
                   
                    Lessons = course.Lessons?.Select(l => new ReadLessonDto
                    {
                        Id = l.Id,
                        Title = l.Title,
                        Content = l.Content,
                        VideoUrl = l.VideoUrl,
                        CourseId = l.CourseId
                    }).ToList() ?? new List<ReadLessonDto>()
                }).ToList();

                _logger.LogInformation("Retrieved {Count} courses for teacher {TeacherName}", courseDtos.Count, teacherName);
                return courseDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses for teacher: {TeacherName}", teacherName);
                throw;
            }
        }

        public async Task<bool> UpdateCourseAsync(int id, CourseUpdateDto courseDto)
        {
            try
            {
                _logger.LogInformation("Updating course with ID: {Id}", id);

                // Check if course exists
                var existingCourse = await _courseRepository.GetCourseAsync(id);
                if (existingCourse == null)
                {
                    _logger.LogWarning("Course not found for update. ID: {Id}", id);
                    return false;
                }

                
                var course = _mapper.Map<Course>(courseDto);
                course.Id = id; 
                
                var result = await _courseRepository.UpdateCourseAsync(id, courseDto);

                if (result)
                {
                    _logger.LogInformation("Course updated successfully. ID: {Id}", id);
                }
                else
                {
                    _logger.LogWarning("Failed to update course. ID: {Id}", id);
                }

                return result;
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
                _logger.LogInformation("Deleting course with ID: {Id}", id);

                
                var existingCourse = await _courseRepository.GetCourseAsync(id);
                if (existingCourse == null)
                {
                    _logger.LogWarning("Course not found for deletion. ID: {Id}", id);
                    return false;
                }

              
                var result = await _courseRepository.DeleteCourseAsync(id);

                if (result)
                {
                    _logger.LogInformation("Course deleted successfully. ID: {Id}", id);
                }
                else
                {
                    _logger.LogWarning("Failed to delete course. ID: {Id}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course with ID: {Id}", id);
                throw;
            }
        }
        public async Task<bool> Exists(int id)
        { 
          return  await _courseRepository.Exists(id);
        }

    }
} 