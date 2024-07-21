using Application.Auth.Request;
using Application.Auth.Response;
using Application.Dto;
using Domain.Ports;
using IdentityAuth.Jwt;
using Microsoft.AspNetCore.Identity;

namespace IdentityAuth;
public class IdentityService : IAuthUserService
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtGenerator _jwtGenerator;

    public IdentityService(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        JwtGenerator jwtGenerator)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtGenerator = jwtGenerator;
    }

    public async Task<LoggedUserResponse> AuthenticateAsync(LoginUserRequest request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, true);
        if (result.Succeeded) return await _jwtGenerator.GerarToken(request.Email);

        var userAuth = new LoggedUserResponse();
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                userAuth.AddError("Your account is blocked");
            else if (result.IsNotAllowed)
                userAuth.AddError("You don't have permission to do this actions");
            else if (result.RequiresTwoFactor)
                userAuth.AddError("You need to confirm 2 factor code");
            else
                userAuth.AddError("User or password is invalid");
        }
        return userAuth;
    }

    public async Task<RegisteredUserResponse> RegisterAsync(RegisterUserRequest register)
    {
        var identityUser = new IdentityUser
        {
            UserName = register.Email,
            Email = register.Email,
            EmailConfirmed = true,
        };
        var result = await _userManager.CreateAsync(identityUser, register.Password);
        var response = new RegisteredUserResponse();
        if (result.Succeeded)
        {
            await _userManager.SetLockoutEnabledAsync(identityUser, false);
            response.UserEmail = register.Email;

            var roleResult = await _userManager.AddToRoleAsync(identityUser, register.Role.ToString());

        }
        else
            response.SetError(result.Errors.ToList().Select(r => r.Description).ToList());

        return response;
    }
}
