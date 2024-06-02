using DemoWeb.Domain.Models;
using DemoWeb.Domain.Utitlities;

namespace DemoWeb.Service.Interfaces
{
    public interface IAuthenticateService
    {
        Task<ApiResult<string>> Register(RegisterModel model);
        Task<ApiResult<UserToken>> Login(LoginModel model);
    }
}
