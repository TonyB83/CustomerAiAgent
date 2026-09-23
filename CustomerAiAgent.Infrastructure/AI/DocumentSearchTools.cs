using CustomerAiAgent.Infrastructure.AI.Documents;
using Microsoft.Extensions.Logging;

namespace CustomerAiAgent.Infrastructure.AI;

public class DocumentSearchTools
{
    private readonly DocumentSearchService _searchService;
    private readonly ILogger<DocumentSearchTools> _logger;

    public DocumentSearchTools(
        DocumentSearchService searchService,
        ILogger<DocumentSearchTools> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    public Task<object> SearchDocuments(
        string query,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "AI Tool Call: search_documents. Query={Query}",
            query);

        if (string.IsNullOrWhiteSpace(query))
        {
            return Task.FromResult<object>(
                new
                {
                    success = false,
                    message =
                        "La recherche documentaire est vide."
                });
        }

        if (query.Length > 500)
        {
            return Task.FromResult<object>(
                new
                {
                    success = false,
                    message =
                        "La recherche est trop longue."
                });
        }

        try
        {
            var documents =
                _searchService.Search(
                    query,
                    maxResults: 3);

            _logger.LogInformation(
                "Document search completed. Query={Query}, Count={Count}",
                query,
                documents.Count);

            return Task.FromResult<object>(
                new
                {
                    success = true,
                    query,
                    count = documents.Count,
                    documents = documents.Select(d => new
                    {
                        d.FileName,
                        d.Content
                    })
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Document search failed. Query={Query}",
                query);

            return Task.FromResult<object>(
                new
                {
                    success = false,
                    message =
                        "Une erreur est survenue pendant la recherche documentaire."
                });
        }
    }
}