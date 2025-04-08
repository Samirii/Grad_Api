using Grad_Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Grad_Api.Repositores
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        private readonly DbSet<Course> _dbSet;

        public CourseRepository(GradProjDbContext context) : base(context)
        {
            _dbSet = context.Set<Course>();
        }

        public async Task<IEnumerable<Course>> GetCoursesByTeacherAsync(string teacherId)
        {
            return await _dbSet
                .Where(c => c.InstructorId.ToString() == teacherId)
                .Include(c => c.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Where(c => c.CategoryId == categoryId)
                .Include(c => c.Category)
                .ToListAsync();
        }

        public async Task<bool> IsTeacherOfCourseAsync(int courseId, string teacherId)
        {
            var course = await _dbSet.FindAsync(courseId);
            return course?.InstructorId.ToString() == teacherId;
        }
    }
} 