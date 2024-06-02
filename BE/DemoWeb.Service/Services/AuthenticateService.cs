using DemoWeb.Domain.Models;
using DemoWeb.Domain.Utitlities;
using DemoWeb.Infrastructure.Constants;
using DemoWeb.Infrastructure.Databases.DemoWebDB;
using DemoWeb.Infrastructure.Databases.DemoWebDB.Entities;
using DemoWeb.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DemoWeb.Service.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly DemoWebDbContext _dbContext;

        public AuthenticateService(
            IConfiguration configuration,
            UserManager<User> userManager,
            SignInManager<User> signInManager, 
            RoleManager<IdentityRole<int>> roleManager,
            DemoWebDbContext dbContext)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbContext= dbContext;
        }

        public async Task<ApiResult<UserToken>> Login(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
                if (signInResult.Succeeded)
                {

                    var role = await _userManager.GetRolesAsync(user);
                    var roleString = String.Join(",", role.ToArray());
                    var tokenResult = GenerateUserToken(user, roleString);
                    if (tokenResult != null)
                    {
                        return new ApiResult<UserToken>()
                        {
                            Data = tokenResult,
                            Message = "Success",
                            StatusCode = 200
                        };
                    }
                }
                return new ApiResult<UserToken>()
                {
                    Data = null,
                    Message = "The password is incorrect",
                    StatusCode = 403
                };
            }
            return new ApiResult<UserToken>()
            {
                Data = null,
                Message = "The Email is not found",
                StatusCode = 403
            };
        }

        public async Task<ApiResult<string>> Register(RegisterModel model)
        {
            var userExist = await _userManager.FindByNameAsync(model.UserName);
            if (userExist != null)
            {
                return new ApiResult<string>()
                {
                    StatusCode = 400,
                    Message = $"{model.Email} đã được sử dụng. Vui lòng thử lại!"
                }; ;
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            
            User user = new User()
            {
                Email = model.Email,
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                SecurityStamp = Guid.NewGuid().ToString(),
        };

            var userCreated = await _userManager.CreateAsync(user, model.Password);
            if (!userCreated.Succeeded)
            {
                return new ApiResult<string>()
                {
                    StatusCode = 500,
                    Message = "Can not create user"
                };
            }

            if (!await _roleManager.RoleExistsAsync(RoleConstants.USER.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole<int>(RoleConstants.USER.ToString()));
            }

            await _userManager.AddToRoleAsync(user, RoleConstants.USER.ToString());

            _dbContext.SaveChanges();

            await transaction.CommitAsync();
            return new ApiResult<string>()
            {
                Message = "Success",
                StatusCode = 200
            };
        }

        private UserToken GenerateUserToken(User user, string role, bool isExternalLogin = false)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var expires = DateTime.UtcNow.AddDays(7);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    //TODO: Add full name
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("userName", user.UserName.ToString()),
                    new Claim("role",role),
                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("isGoogleLogin", isExternalLogin.ToString())
                }),

                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:ValidIssuer"],
                Audience = _configuration["Jwt:ValidAudience"]
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            var token = tokenHandler.WriteToken(securityToken);

            return new UserToken
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = token,
                Expires = expires
            };
        }
    }
}
