using Domain.Enums;

namespace Application.Auth.Request;
public class RegisterUserRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
}
