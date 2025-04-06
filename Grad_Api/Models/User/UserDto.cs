using BookStoreAPI.Models.User;
using System.ComponentModel.DataAnnotations;

namespace Grad_Api.Models.User
{
    public class UserDto : LoginUserDto
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        [Required]

        public string FirstName { get; set; }
        [Required]

        public string LastName { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
