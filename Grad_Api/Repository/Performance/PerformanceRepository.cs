using Grad_Api.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Grad_Api.Repository.Performance
{
    public class PerformanceRepository : IPerformanceRepository
    {
        private readonly GradProjDbContext _context;

        public PerformanceRepository(GradProjDbContext context)
        {
            _context = context;
        }

        public async Task<Course?> GetCourseByIdAsync(int courseId)
        {
            return await _context.Courses.FindAsync(courseId);
        }

        public async Task<List<QuizScore>> GetQuizScoresForStudentInCourseAsync(string studentId, int courseId)
        {
            return await _context.QuizScores
                .Include(q => q.Lesson)
                .Where(q => q.StudentId == studentId && q.Lesson.CourseId == courseId)
                .OrderBy(q => q.Id)
                .ToListAsync();
        }


    }
    
}
