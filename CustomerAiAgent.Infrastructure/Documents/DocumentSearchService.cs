using Microsoft.Extensions.Logging;

namespace CustomerAiAgent.Infrastructure.AI.Documents;

public class DocumentSearchService
{
    private readonly ILogger<DocumentSearchService> _logger;

    private readonly List<Document> _documents = [];

    public DocumentSearchService(
        ILogger<DocumentSearchService> logger)
    {
        _logger = logger;
    }

    public void LoadDocuments(string documentsPath)
    {
        if (!Directory.Exists(documentsPath))
        {
            _logger.LogWarning(
                "Le dossier de documents n'existe pas : {Path}",
                documentsPath);

            return;
        }

        var files = Directory.GetFiles(
            documentsPath,
            "*.txt",
            SearchOption.AllDirectories);

        foreach (var file in files)
        {
            try
            {
                var content = File.ReadAllText(file);

                if (string.IsNullOrWhiteSpace(content))
                {
                    continue;
                }

                var document = new Document
                {
                    Id = _documents.Count + 1,
                    FileName = Path.GetFileName(file),
                    Title = Path.GetFileNameWithoutExtension(file),
                    Content = content
                };

                _documents.Add(document);

                _logger.LogInformation(
                    "Document chargé : {FileName}",
                    document.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erreur lors du chargement du document : {File}",
                    file);
            }
        }

        _logger.LogInformation(
            "{Count} document(s) chargé(s).",
            _documents.Count);
    }

    public List<DocumentSearchResult> Search(
        string query,
        int maxResults = 5)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var queryWords = query
            .Trim()
            .ToLowerInvariant()
            .Split(
                [' ', ',', '.', ';', ':', '?', '!', '\'', '"'],
                StringSplitOptions.RemoveEmptyEntries)
            .Where(word => word.Length >= 3)
            .Distinct()
            .ToList();

        if (queryWords.Count == 0)
        {
            return [];
        }

        _logger.LogInformation(
            "Recherche documentaire : {Query}",
            query);

        var results = new List<DocumentSearchResult>();

        foreach (var document in _documents)
        {
            var searchableText =
                $"{document.Title} {document.Content}"
                    .ToLowerInvariant();

            var matchedWords =
                queryWords.Count(word =>
                    searchableText.Contains(word));

            if (matchedWords == 0)
            {
                continue;
            }

            var score =
                (double)matchedWords / queryWords.Count;

            results.Add(new DocumentSearchResult
            {
                DocumentId = document.Id,
                FileName = document.FileName,
                Title = document.Title,
                Content = document.Content,
                Score = Math.Round(score, 2)
            });
        }

        return results
            .OrderByDescending(result => result.Score)
            .Take(maxResults)
            .ToList();
    }

}