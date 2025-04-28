using Grad_Api.Models.User;

namespace Grad_Api.Services.User
{
    public interface IUserService
    {
        Task<List<UserReadDto>> GetAllUsersAsync();
        Task<UserReadDto?> GetUserByIdAsync(string id);
    }
} 