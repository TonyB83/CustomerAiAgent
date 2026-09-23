namespace CustomerAiAgent.Application.Interfaces;

public interface IAiService
{
    Task<string> AskAsync(
        string question,
        CancellationToken cancellationToken = default);
}