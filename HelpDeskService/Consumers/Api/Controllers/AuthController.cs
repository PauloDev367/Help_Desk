using Api.ViewModels;
using Application.Auth.Request;
using Domain.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/login")]
public class AuthController : ControllerBase
{

    private readonly IAuthUserService _authUserService;

    public AuthController(IAuthUserService authUserService)
    {
        _authUserService = authUserService;
    }

    [HttpPost]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUserVM vm)
    {
        var loginUserRequest = new LoginUserRequest
        {
            Email = vm.Email,
            Password = vm.Password,
        };

        var auth = await _authUserService.AuthenticateAsync(loginUserRequest);
        if (auth.Errors.Count > 0)
        {
            return BadRequest(auth);
        }
        return Ok(auth);
    }

}
