using DemoWeb.Domain.Models;
using DemoWeb.Infrastructure.Databases.DemoWebDB.Entities;
using DemoWeb.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DemoWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticateService _authenticateService;

        public AuthController(IAuthenticateService authenticateService, UserManager<User> userManager)
        {
            _authenticateService = authenticateService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel request)
        {
            try
            {
                var result = await _authenticateService.Login(request);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState.Values.SelectMany(it => it.Errors).Select(it => it.ErrorMessage));
                var result = await _authenticateService.Register(request);

                return new JsonResult(result.Data)
                {
                    StatusCode = result.StatusCode,
                    Value = result.Message
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
