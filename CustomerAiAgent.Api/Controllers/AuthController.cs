using CustomerAiAgent.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerAiAgent.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(
        AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login(
        LoginRequest request)
    {
        var token = _authService.Login(
            request.Username,
            request.Password);

        if (token is null)
        {
            return Unauthorized(new
            {
                message = "Identifiants incorrects."
            });
        }

        return Ok(new
        {
            access_token = token
        });
    }
}

public record LoginRequest(
    string Username,
    string Password);