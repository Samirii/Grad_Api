using AutoMapper;
using AutoMapper.QueryableExtensions;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Microsoft.EntityFrameworkCore;

namespace Grad_Api.Repository
{
    public class CourseRepository : GenericRepository<Course> , ICourseRepository
    {
        private readonly GradProjDbContext context;
        private readonly IMapper mapper;

        public CourseRepository(GradProjDbContext context, IMapper mapper) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            

        }

        public async Task<List<CourseReadDto>> GetAllCoursesAsync()
        {
                return await context.Courses
                .Include(c => c.Category)
                .Include(c => c.Lessons)
                .Include(c => c.Quizzes)
                .ProjectTo<CourseReadDto>(mapper.ConfigurationProvider)
                .ToListAsync();
                }

        public async Task<CourseReadDto> GetCourseAsync(int id)
        {
            var course = await context.Courses
            .Include(c => c.Category)
            .Include(c => c.Lessons)
            .Include(c => c.Quizzes)
            .Where(c => c.Id == id)
            .ProjectTo<CourseReadDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

            if (course == null)
                return null;

            return mapper.Map<CourseReadDto>(course);
        }
        public async Task<List<CourseReadDto>> GetCoursesByCategoryAsync(int categoryId)
        {
            return await context.Courses
                .Include(c => c.Category)
                .Where(c => c.CategoryId == categoryId)
                .ProjectTo<CourseReadDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
        public async Task<CourseCategory> GetCategoryByIdAsync(int? categoryId)
        {
            if (!categoryId.HasValue)
            {
                return null;
            }

            return await context.CourseCategories.FindAsync(categoryId);
        }
    }
}
