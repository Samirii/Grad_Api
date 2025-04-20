using AutoMapper;
using BookStoreAPI.Repositores;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Models.Enrollment;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grad_Api.Repository
{
    public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollmentRepoaitory
    {
        private readonly GradProjDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EnrollmentRepository> _logger;

        public EnrollmentRepository(GradProjDbContext context, IMapper mapper, ILogger<EnrollmentRepository> logger) : base(context)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<EnrollmentReadDto> CreateEnrollment(EnrollmentCreateDto enrollmentCreateDto)
        {
            try
            {
                // Check if student exists
                var student = await _context.Users.FindAsync(enrollmentCreateDto.StudentId);
                if (student == null)
                    throw new Exception("Student not found");

                // Check if course exists
                var course = await _context.Courses.FindAsync(enrollmentCreateDto.CourseId);
                if (course == null)
                    throw new Exception("Course not found");

                // Check if already enrolled
                var existingEnrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.StudentId == enrollmentCreateDto.StudentId && 
                                            e.CourseId == enrollmentCreateDto.CourseId 
                                            );

                if (existingEnrollment != null)
                    throw new Exception("Student is already enrolled in this course");

                var enrollment = new Enrollment
                {
                    
                    StudentId = enrollmentCreateDto.StudentId,
                    CourseId = enrollmentCreateDto.CourseId,
                  
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                return new EnrollmentReadDto
                {
                    Id = enrollment.Id,
                    StudentId = enrollment.StudentId,
                    CourseId = enrollment.CourseId,
                  
                    CourseTitle = course.Title,
                    StudentName = $"{student.FirstName} {student.LastName}"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating enrollment: {ex.Message}");
            }
        }

        public async Task<List<EnrollmentReadDto>> GetStudentEnrollments(string studentId)
        {
            try
            {
                var enrollments = await _context.Enrollments
                    .Include(e => e.Course)
                    .Include(e => e.Student)
                    .Where(e => e.StudentId == studentId )
                    .Select(e => new EnrollmentReadDto
                    {
                        Id = e.Id,
                    
                        StudentId = e.StudentId,
                        CourseId = e.CourseId,
                        
                        
                        CourseTitle = e.Course.Title,
                        StudentName = $"{e.Student.FirstName} {e.Student.LastName}"
                    })
                    .ToListAsync();

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
                var enrollment = await _context.Enrollments.FindAsync(id);
                if (enrollment == null)
                    throw new Exception("Enrollment not found");

                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting enrollment with ID: {Id}", id);
                throw new Exception($"Error deleting enrollment: {ex.Message}");
            }
        }

        public async Task<List<EnrollmentReadDto>> GetAllEnrollments()
        {
            try
            {
                var enrollments = await _context.Enrollments
                    .Include(e => e.Course)
                    .Include(e => e.Student)
                    
                    .Select(e => new EnrollmentReadDto
                    {
                        Id = e.Id,
                        StudentId = e.StudentId,
                        CourseId = e.CourseId,
                      
                        CourseTitle = e.Course.Title,
                        StudentName = $"{e.Student.FirstName} {e.Student.LastName}"
                    })
                    .ToListAsync();

                return enrollments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all enrollments");
                throw new Exception($"Error getting all enrollments: {ex.Message}");
            }
        }

        public async Task<EnrollmentReadDto> GetEnrollment(int id)
        {
            try
            {
                var enrollment = await _context.Enrollments
                    .Include(e => e.Course)
                    .Include(e => e.Student)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (enrollment == null)
                    throw new Exception("Enrollment not found");

                return new EnrollmentReadDto
                {
                    Id = enrollment.Id,
                    StudentId = enrollment.StudentId,
                    CourseId = enrollment.CourseId,                  
                    CourseTitle = enrollment.Course.Title,
                    StudentName = $"{enrollment.Student.FirstName} {enrollment.Student.LastName}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enrollment with ID: {Id}", id);
                throw new Exception($"Error getting enrollment: {ex.Message}");
            }
        }

       
    }
}

