using Grad_Api.Models.User;
using Grad_Api.Repository.User;

namespace Grad_Api.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserReadDto>> GetAllUsersAsync()
        {
            try
            {
                return await _userRepository.GetAllUsersAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all users: {ex.Message}");
            }
        }

        public async Task<UserReadDto?> GetUserByIdAsync(string id)
        {
            try
            {
                return await _userRepository.GetUserByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting user by ID: {ex.Message}");
            }
        }
    }
} 