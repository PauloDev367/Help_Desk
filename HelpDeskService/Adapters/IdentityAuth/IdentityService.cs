using Application.Auth.Request;
using Application.Auth.Response;
using Application.Dto;
using Domain.Entities;
using Domain.Ports;
using IdentityAuth.Exceptions;
using IdentityAuth.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

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

    public async Task UpdateAuthUserAsync(Domain.Entities.User user, UpdateAuthUserRequest request)
    {
        var identityUser = await _userManager.FindByEmailAsync(user.Email);

        if (identityUser == null)
            throw new AuthUserNotFoundException("Auth user was not founded");

        if (!string.IsNullOrEmpty(request.Email))
        {
            identityUser.Email = request.Email;
            identityUser.UserName = request.Email;
            user.Email = request.Email;

            var emailUpdateResult = await _userManager.UpdateAsync(identityUser);
            if (!emailUpdateResult.Succeeded)
                throw new Exception("Error when try to update user");
        }

        if (!string.IsNullOrEmpty(request.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
            var passwordUpdateResult = await _userManager.ResetPasswordAsync(identityUser, token, request.Password);
            if (!passwordUpdateResult.Succeeded)
                throw new Exception("Error when try to update auth user password");
        }

    }
}
