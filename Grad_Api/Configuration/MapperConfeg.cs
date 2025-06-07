using AutoMapper;
using Grad_Api.Data;
using Grad_Api.Models.Course;
using Grad_Api.Models.Lessons;
using Grad_Api.Models.Quiz;
using Grad_Api.Models.User;

namespace Grad_Api.Confeguration
{
    public class MapperConfeg : Profile
    {
        public MapperConfeg()
        { 
            CreateMap<ApiUser, UserDto>().ReverseMap();
            CreateMap<Course, CourseUpdateDto>().ReverseMap();

            CreateMap<Course, CourseReadDto>().ReverseMap();

            CreateMap<Course, CourseReadDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.LessonCount, opt => opt.MapFrom(src => src.Lessons.Count));
                
            CreateMap<ReadLessonDto,Lesson>().ReverseMap();
            CreateMap<CreateLessonDto, Lesson>().ReverseMap();
            CreateMap<QuizCreateDto, Quiz>().ReverseMap();





        }
    }
}
