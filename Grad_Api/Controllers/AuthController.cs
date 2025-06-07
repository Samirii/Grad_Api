using AutoMapper;
using Grad_Api.Models.User;
using Grad_Api.Static;
using Grad_Api.Data;
using Grad_Api.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Grad_Api.Repository.User;

namespace Grad_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IMapper mapper;
        private readonly UserManager<ApiUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly GradProjDbContext context;
        private readonly IUserRepository userRepository;
        private readonly IWebHostEnvironment environment;

        public AuthController(
            ILogger<AuthController> logger,
            IMapper mapper,
            UserManager<ApiUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            GradProjDbContext context,
            IUserRepository userRepository,
            IWebHostEnvironment environment)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.context = context;
            this.userRepository = userRepository;
            this.environment = environment;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] UserDto userDto)
        {
            logger.LogInformation($"Registration Attempt for {userDto.Email}");
            try
            {
                userDto.Role = userDto.Role?.Trim().ToLowerInvariant();
                if (userDto.Role != "teacher" && userDto.Role != "student")
                {
                    return BadRequest("Role must be either 'Teacher' or 'Student'");
                }

                // Validate requirements based on role
                if (userDto.Role == "teacher")
                {
                    // Teacher must have CV
                    if (userDto.CV == null || userDto.CV.Length == 0)
                    {
                        return BadRequest("CV file is required for teacher registration");
                    }

                    // Teacher must have SubjectId
                    if (!userDto.SubjectId.HasValue)
                    {
                        return BadRequest("Subject selection is required for teachers");
                    }

                    // Validate SubjectId range
                    if (userDto.SubjectId < 1 || userDto.SubjectId > 3)
                    {
                        return BadRequest("Invalid subject selected. Subject must be 1 (Math), 2 (Science), or 3 (English)");
                    }
                }
                else // Student role
                {
                    // Students cannot have CV
                    if (userDto.CV != null)
                    {
                        return BadRequest("Students are not allowed to upload CV");
                    }

                    // Students cannot have SubjectId
                    if (userDto.SubjectId.HasValue)
                    {
                        return BadRequest("Students are not allowed to select a subject");
                    }
                }

                var user = mapper.Map<ApiUser>(userDto);
                user.UserName = userDto.Email;
                user.SubjectId = userDto.SubjectId;
                user.PhoneNumber = userDto.PhoneNumber;
                user.PhoneNumberConfirmed = true;
                user.CvFilePath = null; // Ensure CV path is null for students

                var result = await userManager.CreateAsync(user, userDto.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                if (!await roleManager.RoleExistsAsync(userDto.Role))
                {
                    await roleManager.CreateAsync(new IdentityRole(userDto.Role));
                }

                await userManager.AddToRoleAsync(user, char.ToUpper(userDto.Role[0]) + userDto.Role.Substring(1));

                // Handle CV upload for teachers only
                if (userDto.Role == "teacher" && userDto.CV != null)
                {
                    // Create uploads directory in the application root if it doesn't exist
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cvs");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"{user.Id}_{Guid.NewGuid()}{Path.GetExtension(userDto.CV.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await userDto.CV.CopyToAsync(stream);
                    }

                    // Create teacher approval status
               
                }

                // Get subject name for response
                string subjectName = null;
                if (user.SubjectId.HasValue)
                {
                    var subject = await context.Subjects.FindAsync(user.SubjectId.Value);
                    subjectName = subject?.Name;
                }

                var response = new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Role = userDto.Role,
                    SubjectId = user.SubjectId,
                    SubjectName = subjectName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Message = userDto.Role == "teacher" ? "Registration successful. Your application is pending admin approval." : "Registration successful."
                };

                return Accepted(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something Wrong in {nameof(Register)}");
                return Problem(
                    detail: ex.InnerException?.Message ?? ex.Message,
                    title: "An error occurred while registering the user",
                    statusCode: 500);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginUserDto userDto)
        {
            logger.LogInformation($"Login Attempt for {userDto.Email}");
            try
            {
                var user = await userManager.FindByEmailAsync(userDto.Email);
                var PasswordValid = await userManager.CheckPasswordAsync(user, userDto.Password);
                
                if (user == null || PasswordValid == false)
                {
                    return Unauthorized(userDto);
                }

                // Check if user is a teacher and their approval status
                var roles = await userManager.GetRolesAsync(user);
                

                string tokenString = await GenerateToken(user);
                var response = new AuthResponse
                {
                    Email = userDto.Email,
                    Token = tokenString,
                    UserId = user.Id
                };
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something Wrong in {nameof(Login)}");
                return Problem($"Something Wrong in {nameof(Login)}", statusCode: 500);
            }
        }

        private async Task<string> GenerateToken(ApiUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();
            var userClaims = await userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(CustomClaimsTypes.Uid, user.Id),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToInt32(configuration["JwtSettings:Duration"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
