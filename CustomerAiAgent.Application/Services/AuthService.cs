using CustomerAiAgent.Application.Interfaces;

namespace CustomerAiAgent.Application.Services;

public class AuthService
{
    private readonly IJwtService _jwtService;

    public AuthService(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public string? Login(
        string username,
        string password)
    {
        // Démonstration uniquement.
        if (username != "admin" ||
            password != "Password123!")
        {
            return null;
        }

        return _jwtService.GenerateToken(
            userId: 1,
            username: username,
            role: "Admin");
    }
}