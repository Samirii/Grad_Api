using AutoMapper;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Models.Enrollment;
using Grad_Api.Models.Lessons;
using Grad_Api.Services.Enrollment;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Grad_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AllowAnonymous]
    
    public class EnrollmentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILogger<EnrollmentController> _logger;
        public EnrollmentController(IMapper mapper , IEnrollmentService enrollmentService , ILogger<EnrollmentController> logger) {
             _mapper = mapper;
            _enrollmentService = enrollmentService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<List<EnrollmentReadDto>>> GetAllEnrollments()
        {
            var courses = await _enrollmentService.GetAllEnrollmentAsync();
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentReadDto>> GetEnrollment(int id)
        {
            var course = await _enrollmentService.GetEnrollmentAsync(id);
            if (course == null)
                return NotFound();
            return Ok(course);
        }


        [HttpPost]
        public async Task<ActionResult<EnrollmentReadDto>> CreateEnrollment(EnrollmentCreateDto enrollmentCreateDto)
        {
            try
            {
                var enrollment = await _enrollmentService.CreateEnrollmentAsync(enrollmentCreateDto);
                return CreatedAtAction(nameof(GetEnrollment), new { id = enrollment.Id }, enrollment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating enrollment");
                return BadRequest(ex.Message);
            }
        }
         
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var enrollment = await _enrollmentService.GetEnrollmentAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            await _enrollmentService.DeleteEnrollment(id);

            return NoContent();
        }
        [HttpGet("by-Student/{studentId}")]
        public async Task<ActionResult<IEnumerable<EnrollmentReadDto>>> GetEnrollmentByStudentId(string studentId)
        {
            try
            {
                if (string.IsNullOrEmpty(studentId))
                {
                    return BadRequest("Student ID cannot be null or empty");
                }

                var enrollments = await _enrollmentService.GetStudentEnrollments(studentId);
                
                if (enrollments == null || !enrollments.Any())
                {
                    return NotFound($"No enrollments found for student with ID: {studentId}");
                }

                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollments for student {StudentId}", studentId);
                return StatusCode(500, $"Error retrieving enrollments: {ex.Message}");
            }
        }

    }
}
