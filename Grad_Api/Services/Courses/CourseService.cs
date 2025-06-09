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
                _logger.LogInformation("Service: Getting course with ID: {Id}", id);

                var course = await _courseRepository.GetCourseAsync(id);

                if (course == null)
                {
                    _logger.LogWarning("Service: Course not found with ID: {Id}", id);
                    return null;
                }

                var courseDto = _mapper.Map<CourseReadDto>(course);
                _logger.LogInformation("Service: Course mapping successful for ID: {Id}", id);
                return courseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error while getting course with ID: {Id}", id);
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


        public async Task<bool> UpdateCourseAsync(int id, CourseUpdateDto dto)
        {
            try
            {
                _logger.LogInformation("Starting update for Course ID: {Id}", id);

                var course = await _courseRepository.GetCourseAsync(id);
                if (course == null)
                {
                    _logger.LogWarning("Course not found for ID: {Id}", id);
                    return false;
                }

                _mapper.Map(dto, course);  // Maps changes from DTO into entity

                _logger.LogInformation("Mapped successfully, now saving to DB");

                return await _courseRepository.UpdateCourseAsync(id,dto); // Pass entity
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, "AutoMapper failed while updating Course ID: {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating Course ID: {Id}", id);
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

        public async Task<CourseReadDto> CreateCourseAsync(CourseCreateDto courseDto)
        {
            try
            {
                var category = await _courseRepository.GetCategoryByIdAsync(courseDto.CategoryId);

                if (category == null)
                {
                    _logger.LogWarning("Category not found. Defaulting to category ID 1.");
                    category = await _courseRepository.GetCategoryByIdAsync(1);
                    if (category == null)
                        throw new InvalidOperationException("Default category not found.");
                }

                var course = new Course
                {
                    Title = courseDto.Title,
                    Description = courseDto.Description,
                    TeacherName = courseDto.TeacherName,
                    CourseCategoryId = category.Id
                };

                var createdCourse = await _courseRepository.CreateCourseAsync(course);
                var fullCourse = await _courseRepository.GetCourseAsync(createdCourse.Id);

                if (fullCourse == null)
                    throw new InvalidOperationException("Failed to retrieve created course");

                return _mapper.Map<CourseReadDto>(fullCourse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service error while creating course: {Title}", courseDto.Title);
                throw;
            }
        }
    }
}
