using AutoMapper;
using BookStoreAPI;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Models.User;

namespace BookStoreAPI.Confeguration
{
    public class MapperConfeg : Profile
    {
        public MapperConfeg()
        { 
            CreateMap<ApiUser, UserDto>().ReverseMap();
            
            CreateMap<Course, CourseReadDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.LessonCount, opt => opt.MapFrom(src => src.Lessons.Count))
                .ForMember(dest => dest.QuizCount, opt => opt.MapFrom(src => src.Quizzes.Count));
        }
    }
}
