using CustomerAiAgent.Application.DTOs;

namespace CustomerAiAgent.Application.Interfaces;

public interface IDocumentSearchService
{
    Task<DocumentSearchDto> SearchAsync(
        string query,
        CancellationToken cancellationToken = default);
}