using CustomerAiAgent.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace CustomerAiAgent.Infrastructure.AI;

public class GeminiAiService : IAiService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GeminiAiService> _logger;

    public GeminiAiService(
        Kernel kernel,
        IChatCompletionService chatCompletionService,
        IMemoryCache cache,
        ILogger<GeminiAiService> logger)
    {
        _kernel = kernel;
        _chatCompletionService = chatCompletionService;
        _cache = cache;
        _logger = logger;
    }

    public async Task<string> AskAsync(
        string sessionId,
        string question,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return "Veuillez saisir une question.";
        }

        try
        {
            var systemPrompt = """
                Tu es un assistant intelligent de gestion client.

                Tu réponds toujours en français.

                Tu dois être précis et ne jamais inventer
                d'informations provenant de la base de données
                ou des documents internes.

                PLUGIN CUSTOMER

                get_customer
                - Récupère les informations d'un client
                  à partir de son identifiant.

                search_customers
                - Recherche des clients par prénom,
                  nom ou adresse email.

                PLUGIN ORDER

                get_orders
                - Récupère toutes les commandes d'un client.

                get_order
                - Récupère une commande précise.

                get_customer_total
                - Calcule le montant total des commandes
                  d'un client.

                get_orders_by_status
                - Récupère les commandes d'un client
                  selon leur statut.

                PLUGIN DOCUMENT

                search_internal_documents
                - Recherche dans les documents internes
                  de l'entreprise.

                RÈGLES IMPORTANTES

                1. Si la question concerne un client,
                   utilise les fonctions Customer disponibles.

                2. Si l'utilisateur donne un nom,
                   un prénom ou un email sans identifiant,
                   utilise search_customers.

                3. Si la question concerne les commandes,
                   utilise les fonctions Order disponibles.

                4. Si la question demande le montant total
                   d'un client, utilise get_customer_total.

                5. Si la question concerne un statut de commande,
                   utilise get_orders_by_status.

                6. Si la question concerne une procédure,
                   une politique, une règle interne,
                   le SAV, une livraison,
                   une annulation ou un remboursement,
                   utilise search_internal_documents.

                7. Pour toute information provenant des documents
                   internes, utilise d'abord search_internal_documents.

                8. Ne jamais inventer une règle interne.

                9. Si aucune information pertinente n'est trouvée
                   dans les documents internes, indique clairement
                   que l'information n'a pas été trouvée.

                10. Pour une question générale qui ne nécessite
                    ni base de données ni document interne,
                    réponds directement.

                11. Présente les résultats de manière claire
                    et naturelle.

                12. Lorsque search_internal_documents retourne
                    des citations, utilise les informations citées
                    pour construire ta réponse.
                """;

            var cacheKey = $"ChatHistory_{sessionId}";
            if (!_cache.TryGetValue(cacheKey, out ChatHistory? chatHistory) || chatHistory == null)
            {
                chatHistory = new ChatHistory(systemPrompt);
            }

            chatHistory.AddUserMessage(question);

            var settings = new PromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            _logger.LogInformation(
                "AI request received for session {SessionId}: {Question}",
                sessionId, question);

            var result = await _chatCompletionService.GetChatMessageContentAsync(
                chatHistory,
                settings,
                _kernel,
                cancellationToken);

            if (result.Content != null)
            {
                chatHistory.AddMessage(result.Role, result.Content);
            }

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));
            _cache.Set(cacheKey, chatHistory, cacheOptions);

            var answer = result.Content;

            if (string.IsNullOrWhiteSpace(answer))
            {
                _logger.LogWarning("Gemini returned an empty response.");
                return "Je n'ai pas pu générer de réponse.";
            }

            _logger.LogInformation("AI response generated successfully.");
            return answer;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("AI request was cancelled.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing AI request.");
            return "Une erreur s'est produite lors du traitement de votre demande.";
        }
    }
}
