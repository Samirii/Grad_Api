using Grad_Api.Models.Course;
using Grad_Api.Models.Enrollment;

namespace Grad_Api.Services.Enrollment
{
    public interface IEnrollmentService
    {
        Task<EnrollmentReadDto> CreateEnrollmentAsync(EnrollmentCreateDto enrollmentDto);
        Task<List<EnrollmentReadDto>> GetAllEnrollmentAsync();
        Task<EnrollmentReadDto?> GetEnrollmentAsync(int id);
        Task<List<EnrollmentReadDto>> GetStudentEnrollments(string studentId);

        Task<bool> DeleteEnrollment(int id);

    }
}
