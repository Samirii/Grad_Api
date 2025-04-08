using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Grad_Api.Repositores;

namespace Grad_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseController : ControllerBase
    {
        private readonly ILogger<CourseController> _logger;
        private readonly IMapper _mapper;
        private readonly ICourseRepository _courseRepository;

        public CourseController(
            ILogger<CourseController> logger,
            IMapper mapper,
            ICourseRepository courseRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _courseRepository = courseRepository;
        }

        // GET: api/Course
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCourseDto>>> GetCourses()
        {
            var courses = await _courseRepository.GetAllAsync();
            var courseDtos = _mapper.Map<IEnumerable<GetCourseDto>>(courses);
            return Ok(courseDtos);
        }

        // GET: api/Course/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetCourseDto>> GetCourse(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            var courseDto = _mapper.Map<GetCourseDto>(course);
            return Ok(courseDto);
        }

        // POST: api/Course
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<GetCourseDto>> PostCourse(CreateCourseDto createCourseDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = _mapper.Map<Course>(createCourseDto);
            course.InstructorId = int.Parse(userId);

            var createdCourse = await _courseRepository.AddAsync(course);
            var courseDto = _mapper.Map<GetCourseDto>(createdCourse);
            return CreatedAtAction("GetCourse", new { id = courseDto.Id }, courseDto);
        }

        // PUT: api/Course/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> PutCourse(int id, UpdateCourseDto updateCourseDto)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (course.InstructorId.ToString() != userId)
            {
                return Forbid();
            }

            _mapper.Map(updateCourseDto, course);
            await _courseRepository.UpdateAsync(course);

            return NoContent();
        }

        // DELETE: api/Course/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (course.InstructorId.ToString() != userId)
            {
                return Forbid();
            }

            await _courseRepository.DeleteAsync(course);
            return NoContent();
        }

        // GET: api/Course/teacher
        [HttpGet("teacher")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<IEnumerable<GetCourseDto>>> GetTeacherCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courses = await _courseRepository.GetCoursesByTeacherAsync(userId);
            var courseDtos = _mapper.Map<IEnumerable<GetCourseDto>>(courses);
            return Ok(courseDtos);
        }

        // GET: api/Course/category/{categoryId}
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<GetCourseDto>>> GetCoursesByCategory(int categoryId)
        {
            var courses = await _courseRepository.GetCoursesByCategoryAsync(categoryId);
            var courseDtos = _mapper.Map<IEnumerable<GetCourseDto>>(courses);
            return Ok(courseDtos);
        }
    }
} 