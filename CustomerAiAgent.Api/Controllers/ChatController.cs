using CustomerAiAgent.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerAiAgent.Api.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IAiService _aiService;

    public ChatController(
        IAiService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost]
    public async Task<IActionResult> Chat(
        ChatRequest request,
        CancellationToken cancellationToken)
    {
        var sessionId = request.SessionId;
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            sessionId = Guid.NewGuid().ToString();
        }

        var answer =
            await _aiService.AskAsync(
                sessionId,
                request.Message,
                cancellationToken);

        return Ok(new
        {
            sessionId,
            answer
        });
    }
}

public record ChatRequest(string? SessionId, string Message);