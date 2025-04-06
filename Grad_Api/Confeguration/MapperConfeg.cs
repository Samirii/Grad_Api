using AutoMapper;
using BookStoreAPI;

using Grad_Api.Data;
using Grad_Api.Models.User;


namespace BookStoreAPI.Confeguration
{
    public class MapperConfeg : Profile
    {
        public MapperConfeg()
        { 
           
            CreateMap<ApiUser, UserDto>().ReverseMap();

        }

    }
}
