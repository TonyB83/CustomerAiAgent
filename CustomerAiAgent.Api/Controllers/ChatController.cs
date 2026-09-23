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
        var answer =
            await _aiService.AskAsync(
                request.Message,
                cancellationToken);

        return Ok(new
        {
            answer
        });
    }
}

public record ChatRequest(string Message);