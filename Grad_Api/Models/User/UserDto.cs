using BookStoreAPI.Models.User;
using System.ComponentModel;
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

        // For teacher registration only
        [Range(1, 3, ErrorMessage = "Subject ID must be between 1 and 3")]
        public int? SubjectId { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Phone number must be between 10 and 15 digits")]
        public string PhoneNumber { get; set; }
    }
}
