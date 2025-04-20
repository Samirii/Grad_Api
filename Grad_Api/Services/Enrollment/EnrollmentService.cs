using AutoMapper;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Models.Enrollment;
using Grad_Api.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grad_Api.Services.Enrollment
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepoaitory _enrollmentRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EnrollmentService(
            IEnrollmentRepoaitory enrollmentRepository, 
            IMapper mapper,
            UserManager<ApiUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _enrollmentRepository = enrollmentRepository;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<EnrollmentReadDto> CreateEnrollmentAsync(EnrollmentCreateDto enrollmentDto)
        {
            try
            {
                // Check if user exists and is a student
                var user = await _userManager.FindByIdAsync(enrollmentDto.StudentId);
                if (user == null)
                    throw new Exception("User not found");

                var userRoles = await _userManager.GetRolesAsync(user);
                if (!userRoles.Contains("Student"))
                    throw new Exception("Only students can enroll in courses");

                var enrollment = await _enrollmentRepository.CreateEnrollment(enrollmentDto);
                return enrollment;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating enrollment: {ex.Message}");
            }
        }

        public async Task<List<EnrollmentReadDto>> GetAllEnrollmentAsync()
        {

            try
            {
                var enrollments = await _enrollmentRepository.GetAllEnrollments();
                return enrollments;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all enrollments: {ex.Message}");
            }
        }

        public async Task<EnrollmentReadDto?> GetEnrollmentAsync(int id)
        {
            try
            {
                var enrollment = await _enrollmentRepository.GetEnrollment(id);
                return enrollment;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting enrollment: {ex.Message}");
            }
        }

        public async Task<List<EnrollmentReadDto>> GetStudentEnrollments(string studentId)
        {
            try
            {
                var enrollments = await _enrollmentRepository.GetStudentEnrollments(studentId);
                return enrollments;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting student enrollments: {ex.Message}");
            }
        }

        public async Task<bool> DeleteEnrollment(int id)
        {
            try
            {
                var result = await _enrollmentRepository.DeleteEnrollment(id);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting enrollment: {ex.Message}");
            }
        }

        public async Task<ActionResult<IEnumerable<EnrollmentReadDto>>> GetEnrollmentByStudentId(string studentId)
        {
            try
            {
                if (string.IsNullOrEmpty(studentId))
                {
                    return new BadRequestObjectResult("Student ID cannot be null or empty");
                }

                var enrollments = await _enrollmentRepository.GetStudentEnrollments(studentId);
                
                if (enrollments == null || !enrollments.Any())
                {
                    return new NotFoundObjectResult($"No enrollments found for student with ID: {studentId}");
                }

                return new OkObjectResult(enrollments);
            }
            catch (Exception ex)
            {
                return new ObjectResult($"Error retrieving enrollments: {ex.Message}")
                {
                    StatusCode = 500
                };
            }
        }
    }
}
