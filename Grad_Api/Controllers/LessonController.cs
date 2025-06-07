using AutoMapper;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Models.Lessons;
using Grad_Api.Services.Lesson;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Grad_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        private readonly ILogger<LessonController> _logger;
        private readonly IMapper _mapper;

        public LessonController(
            ILessonService lessonService,
            ILogger<LessonController> logger,
            IMapper mapper)
        {
            _lessonService = lessonService;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadLessonDto>>> GetLessons()
        {
            try
            {
                var lessons = await _lessonService.GetAllLessonsAsync();
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all Lessons");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReadLessonDto>> GetLessonById(int id)
        {
            try
            {
                var lesson = await _lessonService.GetLessonAsync(id);

                if (lesson == null)
                {
                    return NotFound($"Lesson with ID {id} not found");
                }

                return Ok(lesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting lesson with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }




        [HttpPost]
        [Authorize(Roles = "Administrator")]

        public async Task<ActionResult<ReadLessonDto>> CreateLesson(CreateLessonDto lessonDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid ModelState: {@ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Creating lesson: {@LessonDto}", lessonDto);

               
                var courseExists = await _lessonService.CourseExistsAsync(lessonDto.CourseId);
                if (!courseExists)
                {
                    _logger.LogWarning($"Course with ID {lessonDto.CourseId} not found");
                    return BadRequest($"Course with ID {lessonDto.CourseId} does not exist");
                }

                
                var lesson = _mapper.Map<Lesson>(lessonDto);
                _logger.LogInformation("Mapped lesson entity: {@Lesson}", lesson);

                try
                {
                    var createdLesson = await _lessonService.CreateLessonAsync(lessonDto);
                    var lessonReadDto = _mapper.Map<ReadLessonDto>(createdLesson);
                    return CreatedAtAction(nameof(GetLessonById), new { id = createdLesson.Id }, lessonReadDto);
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error details: {@Error}, Inner exception: {@InnerError}", 
                        ex.Message, ex.InnerException?.Message);
                    return BadRequest($"Database error: {ex.InnerException?.Message ?? ex.Message}");
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning(ex, "Validation error: {Error}", ex.Message);
                    return BadRequest(ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating lesson: {Error}", ex.Message);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]

        public async Task<IActionResult> DeleteLesson(int id)
        {
            var lesson = await _lessonService.GetLessonAsync(id);
            if (lesson == null)
            {
                return NotFound($"Lesson with ID {id} not found");
            }
            await _lessonService.DeleteLessonAsync(id);

            return NoContent();


        }

        [HttpGet("by-course/{courseId}")]

        public async Task<ActionResult<IEnumerable<ReadLessonDto>>> GetLessonsByCourseId(int courseId)
        {
            try
            {
                var lessons = await _lessonService.GetLessonsByCourseIdAsync(courseId);

                if (lessons == null || !lessons.Any())
                {
                    return NotFound($"No lessons found for course ID {courseId}");
                }

                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting lessons for course ID {courseId}");
                return StatusCode(500, "Internal server error");
            }
        }


    }
}