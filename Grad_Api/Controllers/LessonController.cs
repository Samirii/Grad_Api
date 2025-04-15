﻿using AutoMapper;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Models.Lessons;
using Grad_Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Grad_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ILogger<LessonController> _logger;
        private readonly IMapper _mapper;

        public LessonController(
            ILessonRepository lessonRepository,
            ILogger<LessonController> logger,
            IMapper mapper)
        {
            _lessonRepository = lessonRepository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadLessonDto>>> GetLessons()
        {
            try
            {
                var lessons = await _lessonRepository.GetLessons();
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
                var lesson = await _lessonRepository.GetAsync(id);

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

               
                var courseExists = await _lessonRepository.CourseExistsAsync(lessonDto.CourseId);
                if (!courseExists)
                {
                    _logger.LogWarning($"Course with ID {lessonDto.CourseId} not found");
                    return BadRequest($"Course with ID {lessonDto.CourseId} does not exist");
                }

                
                var lesson = _mapper.Map<Lesson>(lessonDto);
                _logger.LogInformation("Mapped lesson entity: {@Lesson}", lesson);

                try
                {
                    var createdLesson = await _lessonRepository.AddAsync(lesson);
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
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var lesson = await _lessonRepository.GetAsync(id);
            if (lesson == null)
            {
                return NotFound($"Lesson with ID {id} not found");
            }
            await _lessonRepository.DeleteAsync(id);

            return NoContent();


        }
     

    }
}