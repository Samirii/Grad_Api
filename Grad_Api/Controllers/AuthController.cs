using AutoMapper;
using BookStoreAPI.Models.User;
using BookStoreAPI.Static;
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



        public AuthController(ILogger<AuthController> logger, IMapper mapper, UserManager<ApiUser> userManager, RoleManager<IdentityRole> roleManager,IConfiguration configuration)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            logger.LogInformation($"Registration Attempt for {userDto.Email}");
            try
            {
                userDto.Role = userDto.Role?.Trim().ToLowerInvariant();
                if (userDto.Role != "teacher" && userDto.Role != "student")
                {
                    return BadRequest("Role must be either 'Teacher' or 'Student'");
                }
                var user = mapper.Map<ApiUser>(userDto);
                user.UserName = userDto.Email;
               
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
                    return BadRequest($"Role '{userDto.Role}' does not exist.");
                }


                await userManager.AddToRoleAsync(user, char.ToUpper(userDto.Role[0]) + userDto.Role.Substring(1));
             
                return Accepted(new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Role = userDto.Role,
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Something Wrong{nameof(Register)}");
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
                logger.LogInformation($"Registration Attempt for {userDto.Email}");
                try
                {

                    var user = await userManager.FindByEmailAsync(userDto.Email);
                var PasswordValid = await userManager.CheckPasswordAsync(user, userDto.Password);
                    if (user == null || PasswordValid == false)
                    {
                        return Unauthorized(userDto);
                    }

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
                    logger.LogError(ex, $"Something Wrong{nameof(Register)}");
                    return Problem($"Somthing Wrong  in {nameof(Register)}", statusCode: 500);
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
                    new Claim(JwtRegisteredClaimNames.Sub , user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email , user.Email),
                    new Claim(CustomClaimsTypes.Uid,user.Id),

                }
                .Union(userClaims)
                .Union(roleClaims);

                var tolen = new JwtSecurityToken(
                    issuer: configuration["JwtSettings:Issuer"],
                    audience: configuration["JwtSettings:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(Convert.ToInt32(configuration["JwtSettings:Duration"])),
                    signingCredentials: credentials


                );
                return new JwtSecurityTokenHandler().WriteToken(tolen);

            }


    }
    }

