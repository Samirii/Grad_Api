using Grad_Api.Models.User;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Grad_Api.Models.User
{
    public class UserDto : LoginUserDto
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
    ErrorMessage = "Invalid email format. Example: example@domain.com")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$",
     ErrorMessage = "Password must be at least 6 characters long and include an uppercase letter, a lowercase letter, a number, and a special character.")]
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

        // CV is optional and will be validated in the controller based on role
        [ValidateCVBasedOnRole]
        public IFormFile? CV { get; set; }
    }

    // Custom validation attribute for CV based on role
    public class ValidateCVBasedOnRole : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var userDto = (UserDto)validationContext.ObjectInstance;
            
            if (userDto.Role?.ToLower() == "student" && value != null)
            {
                return new ValidationResult("Students are not allowed to upload CV");
            }
            
            if (userDto.Role?.ToLower() == "teacher" && value == null)
            {
                return new ValidationResult("CV is required for teacher registration");
            }

            return ValidationResult.Success;
        }
    }
}
