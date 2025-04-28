using Grad_Api.Models.User;

namespace Grad_Api.Repository.User
{
    public interface IUserRepository
    {
        Task<List<UserReadDto>> GetAllUsersAsync();
        Task<UserReadDto?> GetUserByIdAsync(string id);
    }
}
