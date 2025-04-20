using Grad_Api.Repositores;
using Grad_Api.Data;
using Grad_Api.Models.Enrollment;
using Grad_Api.Models.Course;

namespace Grad_Api.Repository
{
    public interface IEnrollmentRepoaitory:IGenericRepository<Enrollment>
    {
        Task<List<EnrollmentReadDto>> GetAllEnrollments();
        Task<EnrollmentReadDto> GetEnrollment(int id);  
        Task<List<EnrollmentReadDto>> GetStudentEnrollments(string studentId);    
        Task<EnrollmentReadDto> CreateEnrollment(EnrollmentCreateDto enrollmentCreateDto);
        Task<bool> DeleteEnrollment(int id);
    }
}
