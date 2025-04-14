using AutoMapper;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Grad_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<CourseController> _logger;
        private readonly IMapper _mapper;

        public CourseController(
            ICourseRepository courseRepository,
            ILogger<CourseController> logger,
            IMapper mapper)
        {
            _courseRepository = courseRepository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseReadDto>>> GetAllCourses()
        {
            try
            {
                var courses = await _courseRepository.GetAllAsync();
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all courses");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseReadDto>> GetCourseById(int id)
        {
            try
            {
                var course = await _courseRepository.GetAsync(id);

                if (course == null)
                {
                    return NotFound($"Course with ID {id} not found");
                }

                return Ok(course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting course with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CourseReadDto>> PostCourse(CourseCreateDto courseDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get the category from the database
                var category = await _courseRepository.GetCourseCategoryAsync(courseDto.CategoryId);
                if (category == null)
                {
                    return BadRequest($"Category with ID {courseDto.CategoryId} not found");
                }

                var course = new Course
                {
                    Title = courseDto.Title,
                    Description = courseDto.Description,
                    TeacherName = courseDto.TeacherName,
                    CategoryId = courseDto.CategoryId
                };

                await _courseRepository.AddAsync(course);

                var courseReadDto = _mapper.Map<CourseReadDto>(course);
                return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, courseReadDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new course");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _courseRepository.GetAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            await _courseRepository.DeleteAsync(id); 

            return NoContent();


        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, CourseUpdateDto courseDto)
        {
            if (id != courseDto.Id)
            {
                return BadRequest();
            }
            var course = await _courseRepository.GetAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            _mapper.Map(courseDto, course);
            try
            {
                await _courseRepository.UpdateAsync(course);
            }
            catch
            {

                if (!await CourseExiste(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }
            return NoContent();
        
        }
        private async Task<bool> CourseExiste(int id)
        {
            return await _courseRepository.Exists(id);
        }

    }
}
