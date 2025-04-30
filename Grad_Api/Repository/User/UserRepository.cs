using AutoMapper;
using Grad_Api.Data;
using Grad_Api.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Grad_Api.Repository.User
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly GradProjDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(
            UserManager<ApiUser> userManager,
            RoleManager<IdentityRole> roleManager,
            GradProjDbContext context,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserReadDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users
                .Include(u => u.Subject)
                .ToListAsync();

            var userDtos = new List<UserReadDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();

                var userDto = new UserReadDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = role,
                    SubjectId = user.SubjectId,
                    SubjectName = user.Subject?.Name,
                    PhoneNumber = user.PhoneNumber
                };

                userDtos.Add(userDto);
            }

            return userDtos;
        }

        public async Task<UserReadDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.Subject)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            return new UserReadDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = role,
                SubjectId = user.SubjectId,
                SubjectName = user.Subject?.Name,
                PhoneNumber = user.PhoneNumber
            };
        }

      
    }
}
