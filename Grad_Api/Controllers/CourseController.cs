﻿using AutoMapper;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Repository;
using Grad_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Grad_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CourseController> _logger;
        private readonly IMapper _mapper;

        public CourseController(
            ICourseService courseService,
            ILogger<CourseController> logger,
            IMapper mapper)
        {
            _courseService = courseService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CourseReadDto>>> GetAllCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseReadDto>> GetCourse(int id)
        {
            var course = await _courseService.GetCourseAsync(id);
            if (course == null)
                return NotFound();
            return Ok(course);
        }

        [HttpPost]
        public async Task<ActionResult<CourseReadDto>> CreateCourse(CourseCreateDto courseDto)
        {
            var course = await _courseService.CreateCourseAsync(courseDto);
            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _courseService.GetCourseAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            await _courseService.DeleteCourseAsync(id); 

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, CourseUpdateDto courseDto)
        {
            if (id != courseDto.Id)
            {
                return BadRequest();
            }
            var course = await _courseService.GetCourseAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            _mapper.Map(courseDto, course);
            try
            {
                await _courseService.UpdateCourseAsync(id, courseDto);
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

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<List<CourseReadDto>>> GetCoursesByCategory(int categoryId)
        {
            var courses = await _courseService.GetCoursesByCategoryAsync(categoryId);
            return Ok(courses);
        }


        private async Task<bool> CourseExiste(int id)
        {
            return await _courseService.Exists(id);
           
        }
    }
}
