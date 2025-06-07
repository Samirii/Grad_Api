using System.ComponentModel.DataAnnotations;
namespace Grad_Api.Models.User
{
    public class LoginUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
      
    }
}