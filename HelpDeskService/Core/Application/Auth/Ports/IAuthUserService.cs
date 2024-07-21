using Application.Auth.Request;
using Application.Auth.Response;

namespace Domain.Ports;
public interface IAuthUserService
{
    public Task<LoggedUserResponse> AuthenticateAsync(LoginUserRequest request);
    public Task<RegisteredUserResponse> RegisterAsync(RegisterUserRequest register);
}
