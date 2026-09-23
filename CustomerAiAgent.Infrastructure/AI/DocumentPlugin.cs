using CustomerAiAgent.Application.DTOs;
using CustomerAiAgent.Application.Interfaces;
using CustomerAiAgent.Infrastructure.AI.Documents;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace CustomerAiAgent.Infrastructure.AI;

public class DocumentPlugin
{
    private readonly IDocumentSearchService _documentSearchService;
    private readonly ILogger<DocumentPlugin> _logger;

    public DocumentPlugin(
        IDocumentSearchService documentSearchService,
        ILogger<DocumentPlugin> logger)
    {
        _documentSearchService = documentSearchService;
        _logger = logger;
    }

    [KernelFunction("search_internal_documents")]
    [Description(
        "Recherche des informations dans les documents internes " +
        "de l'entreprise. Utilise cette fonction pour les procédures, " +
        "règles internes, remboursements, SAV, annulations, " +
        "livraisons ou autres informations présentes dans les documents.")]
    public async Task<DocumentSearchDto>
    SearchInternalDocuments(
        string query,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Recherche documentaire : {Query}",
            query);

        return await _documentSearchService.SearchAsync(
            query,
            cancellationToken);
    }
}