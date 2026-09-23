using CustomerAiAgent.Application.DTOs;
using CustomerAiAgent.Application.Interfaces;
using Google.Apis.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomerAiAgent.Infrastructure.AI.Documents;

public class GeminiFileSearchService: IDocumentSearchService
{
    private const string ApiBaseUrl =
        "https://generativelanguage.googleapis.com";

    private const string ApiVersion = "v1beta";

    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiFileSearchService> _logger;
    private readonly System.Net.Http.IHttpClientFactory _httpClientFactory;

    private string? _storeName;

    public GeminiFileSearchService(
        System.Net.Http.IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<GeminiFileSearchService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }
    private HttpClient HttpClient => _httpClientFactory.CreateClient();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    // ============================================================
    // INITIALISATION
    // ============================================================

    public async Task InitializeAsync(
        string documentsPath,
        CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(documentsPath))
        {
            throw new DirectoryNotFoundException(
                $"Le dossier des documents n'existe pas : {documentsPath}");
        }

        _logger.LogInformation(
            "Initialisation Gemini File Search : {Path}",
            documentsPath);

        _storeName =
            await GetOrCreateStoreAsync(
                cancellationToken);

        _logger.LogInformation(
            "File Search Store utilisé : {StoreName}",
            _storeName);

        var files = Directory.GetFiles(
            documentsPath,
            "*.txt",
            SearchOption.AllDirectories);

        _logger.LogInformation(
            "{Count} fichier(s) TXT trouvé(s).",
            files.Length);

        if (files.Length == 0)
        {
            _logger.LogWarning(
                "Aucun fichier TXT trouvé dans {Path}",
                documentsPath);

            return;
        }

        /*
         * Pour éviter de réindexer tous les fichiers à chaque
         * démarrage, on utilise un fichier local contenant
         * la liste des fichiers déjà indexés.
         */
        var indexedFilesPath =
            Path.Combine(
                documentsPath,
                ".gemini-indexed-files.json");

        var indexedFiles =
            await LoadIndexedFilesAsync(
                indexedFilesPath,
                cancellationToken);

        foreach (var file in files)
        {
            var fileName =
                Path.GetFileName(file);

            if (indexedFiles.Contains(fileName))
            {
                _logger.LogInformation(
                    "Fichier déjà indexé : {FileName}",
                    fileName);

                continue;
            }

            _logger.LogInformation(
                "Indexation du fichier : {FileName}",
                fileName);

            await UploadFileAsync(
                file,
                cancellationToken);

            indexedFiles.Add(fileName);

            await SaveIndexedFilesAsync(
                indexedFilesPath,
                indexedFiles,
                cancellationToken);
        }

        _logger.LogInformation(
            "Initialisation File Search terminée.");
    }

    // ============================================================
    // STORE
    // ============================================================

    private async Task<string> GetOrCreateStoreAsync(
        CancellationToken cancellationToken)
    {
        var configuredName =
            _configuration["Gemini:FileSearchStore"];

        if (string.IsNullOrWhiteSpace(configuredName))
        {
            configuredName =
                "customer-ai-agent-documents";
        }

        var stores =
            await ListStoresAsync(
                cancellationToken);

        var existingStore =
            stores.FirstOrDefault(
                store =>
                    string.Equals(
                        store.DisplayName,
                        configuredName,
                        StringComparison.OrdinalIgnoreCase));

        if (existingStore is not null)
        {
            _logger.LogInformation(
                "File Search Store existant trouvé : {Name}",
                existingStore.Name);

            return existingStore.Name!;
        }

        _logger.LogInformation(
            "Création du File Search Store : {DisplayName}",
            configuredName);

        var url =
            $"{ApiBaseUrl}/{ApiVersion}/fileSearchStores";

        var requestBody = new
        {
            displayName = configuredName,

            // Modèle d'embedding utilisé par File Search.
            embeddingModel = "models/gemini-embedding-001"
        };

        using var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                url);

        AddApiKey(request);

        request.Content =
            new StringContent(
                JsonSerializer.Serialize(
                    requestBody,
                    JsonOptions),
                Encoding.UTF8,
                "application/json");

        using var response =
            await HttpClient.SendAsync(
                request,
                cancellationToken);

        var json =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Erreur création File Search Store : " +
                $"{response.StatusCode} - {json}");
        }

        var store =
            JsonSerializer.Deserialize<FileSearchStoreResponse>(
                json,
                JsonOptions);

        if (string.IsNullOrWhiteSpace(store?.Name))
        {
            throw new InvalidOperationException(
                "Gemini n'a pas retourné le nom du File Search Store.");
        }

        return store.Name;
    }

    private async Task<List<FileSearchStoreResponse>>
        ListStoresAsync(
            CancellationToken cancellationToken)
    {
        var url =
            $"{ApiBaseUrl}/{ApiVersion}/fileSearchStores";

        using var request =
            new HttpRequestMessage(
                HttpMethod.Get,
                url);

        AddApiKey(request);

        using var response =
            await HttpClient.SendAsync(
                request,
                cancellationToken);

        var json =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Erreur récupération File Search Stores : " +
                $"{response.StatusCode} - {json}");
        }

        var result =
            JsonSerializer.Deserialize<
                FileSearchStoreListResponse>(
                json,
                JsonOptions);

        return result?.FileSearchStores ?? [];
    }

    // ============================================================
    // UPLOAD
    // ============================================================

    private async Task UploadFileAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_storeName))
        {
            throw new InvalidOperationException(
                "File Search Store non initialisé.");
        }

        var fileName =
            Path.GetFileName(filePath);

        var fileBytes =
            await File.ReadAllBytesAsync(
                filePath,
                cancellationToken);

        var uploadUrl =
            $"{ApiBaseUrl}/upload/{ApiVersion}/" +
            $"{_storeName}:uploadToFileSearchStore";

        // --------------------------------------------------------
        // 1. Démarrage de l'upload resumable
        // --------------------------------------------------------

        using var startRequest =
            new HttpRequestMessage(
                HttpMethod.Post,
                uploadUrl);

        AddApiKey(startRequest);

        startRequest.Headers.Add(
            "X-Goog-Upload-Protocol",
            "resumable");

        startRequest.Headers.Add(
            "X-Goog-Upload-Command",
            "start");

        startRequest.Headers.Add(
            "X-Goog-Upload-Header-Content-Length",
            fileBytes.Length.ToString());

        startRequest.Headers.Add(
            "X-Goog-Upload-Header-Content-Type",
            "text/plain");

        var metadata = new
        {
            displayName = fileName
        };

        startRequest.Content =
            new StringContent(
                JsonSerializer.Serialize(
                    metadata,
                    JsonOptions),
                Encoding.UTF8,
                "application/json");

        using var startResponse =
            await HttpClient.SendAsync(
                startRequest,
                cancellationToken);

        if (!startResponse.IsSuccessStatusCode)
        {
            var error =
                await startResponse.Content
                    .ReadAsStringAsync(
                        cancellationToken);

            throw new InvalidOperationException(
                $"Erreur démarrage upload '{fileName}' : " +
                $"{startResponse.StatusCode} - {error}");
        }

        if (!startResponse.Headers.TryGetValues(
                "X-Goog-Upload-URL",
                out var uploadUrls))
        {
            throw new InvalidOperationException(
                "Gemini n'a pas retourné X-Goog-Upload-URL.");
        }

        var resumableUrl =
            uploadUrls.First();

        // --------------------------------------------------------
        // 2. Upload des bytes + finalize
        // --------------------------------------------------------

        using var uploadRequest =
            new HttpRequestMessage(
                HttpMethod.Post,
                resumableUrl);

        uploadRequest.Headers.Add(
            "X-Goog-Upload-Offset",
            "0");

        uploadRequest.Headers.Add(
            "X-Goog-Upload-Command",
            "upload, finalize");

        uploadRequest.Content =
            new ByteArrayContent(fileBytes);

        uploadRequest.Content.Headers.ContentType =
            new MediaTypeHeaderValue(
                "text/plain");

        uploadRequest.Content.Headers.ContentLength =
            fileBytes.Length;

        using var uploadResponse =
            await HttpClient.SendAsync(
                uploadRequest,
                cancellationToken);

        var uploadJson =
            await uploadResponse.Content
                .ReadAsStringAsync(
                    cancellationToken);

        if (!uploadResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Erreur upload '{fileName}' : " +
                $"{uploadResponse.StatusCode} - {uploadJson}");
        }

        var operation =
            JsonSerializer.Deserialize<
                GeminiOperationResponse>(
                uploadJson,
                JsonOptions);

        if (operation is null)
        {
            throw new InvalidOperationException(
                $"Réponse d'indexation invalide pour '{fileName}'.");
        }

        // --------------------------------------------------------
        // 3. Attendre l'indexation
        // --------------------------------------------------------

        await WaitForOperationAsync(
            operation,
            cancellationToken);

        _logger.LogInformation(
            "Fichier indexé avec succès : {FileName}",
            fileName);
    }

    // ============================================================
    // ATTENTE OPERATION
    // ============================================================

    private async Task WaitForOperationAsync(
        GeminiOperationResponse operation,
        CancellationToken cancellationToken)
    {
        if (operation.Done)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(operation.Name))
        {
            throw new InvalidOperationException(
                "L'opération Gemini ne possède pas de nom.");
        }

        var operationUrl =
            $"{ApiBaseUrl}/{ApiVersion}/{operation.Name}";

        while (!operation.Done)
        {
            await Task.Delay(
                TimeSpan.FromSeconds(3),
                cancellationToken);

            using var request =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    operationUrl);

            AddApiKey(request);

            using var response =
                await HttpClient.SendAsync(
                    request,
                    cancellationToken);

            var json =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Erreur pendant l'indexation : " +
                    $"{response.StatusCode} - {json}");
            }

            operation =
                JsonSerializer.Deserialize<
                    GeminiOperationResponse>(
                    json,
                    JsonOptions)
                ?? throw new InvalidOperationException(
                    "Réponse d'opération invalide.");
        }

        if (operation.Error is not null)
        {
            throw new InvalidOperationException(
                $"Erreur indexation Gemini : " +
                $"{operation.Error.Message}");
        }
    }

    // ============================================================
    // RECHERCHE
    // ============================================================

    public async Task<DocumentSearchDto>
        SearchAsync(
            string question,
            CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            throw new ArgumentException(
                "La question est obligatoire.",
                nameof(question));
        }

        if (string.IsNullOrWhiteSpace(_storeName))
        {
            throw new InvalidOperationException(
                "Gemini File Search n'est pas initialisé.");
        }

        var model =
            _configuration["Gemini:Model"]
            ?? "gemini-flash-latest";

        var url =
            $"{ApiBaseUrl}/{ApiVersion}/interactions";

        var requestBody = new
        {
            model,

            input = question,

            tools = new[]
            {
                new
                {
                    type = "file_search",

                    file_search_store_names =
                        new[]
                        {
                            _storeName
                        },

                    top_k = 5
                }
            }
        };

        using var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                url);

        AddApiKey(request);

        request.Content =
            new StringContent(
                JsonSerializer.Serialize(
                    requestBody,
                    JsonOptions),
                Encoding.UTF8,
                "application/json");

        using var response =
            await HttpClient.SendAsync(
                request,
                cancellationToken);

        var json =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Erreur Gemini File Search : " +
                $"{response.StatusCode} - {json}");
        }

        return ParseSearchResponse(json);
    }

    // ============================================================
    // PARSE REPONSE + CITATIONS
    // ============================================================

    private DocumentSearchDto ParseSearchResponse(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        var result = new DocumentSearchDto
        {
            Answer = string.Empty,
            Citations = []
        };

        // Récupération de la réponse
        if (root.TryGetProperty("answer", out var answerElement))
        {
            result.Answer = answerElement.GetString() ?? string.Empty;
        }

        // Récupération des citations
        if (root.TryGetProperty("citations", out var citationsElement) &&
            citationsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var citation in citationsElement.EnumerateArray())
            {
                var citationDto = new DocumentCitationDto
                {
                    FileName = citation.TryGetProperty("fileName", out var fileName)
                        ? fileName.GetString() ?? string.Empty
                        : string.Empty,

                    Source = citation.TryGetProperty("source", out var source)
                        ? source.GetString() ?? string.Empty
                        : string.Empty,

                    PageNumber = citation.TryGetProperty("pageNumber", out var pageNumber) &&
                                 pageNumber.ValueKind == JsonValueKind.Number
                        ? pageNumber.GetInt32()
                        : null
                };

                result.Citations.Add(citationDto);
            }
        }

        return result;
    }

    // ============================================================
    // INDEX LOCAL
    // ============================================================

    private async Task<HashSet<string>>
        LoadIndexedFilesAsync(
            string path,
            CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
        {
            return [];
        }

        try
        {
            var json =
                await File.ReadAllTextAsync(
                    path,
                    cancellationToken);

            var files =
                JsonSerializer.Deserialize<List<string>>(
                    json,
                    JsonOptions);

            return files is null
                ? []
                : files.ToHashSet(
                    StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            return [];
        }
    }

    private async Task SaveIndexedFilesAsync(
        string path,
        HashSet<string> files,
        CancellationToken cancellationToken)
    {
        var json =
            JsonSerializer.Serialize(
                files.OrderBy(x => x).ToList(),
                JsonOptions);

        await File.WriteAllTextAsync(
            path,
            json,
            cancellationToken);
    }

    // ============================================================
    // API KEY
    // ============================================================

    private void AddApiKey(
        HttpRequestMessage request)
    {
        var apiKey =
            _configuration["Gemini:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "Gemini:ApiKey est manquante.");
        }

        request.Headers.Add(
            "x-goog-api-key",
            apiKey);
    }

    // ============================================================
    // DTO REST
    // ============================================================

    private class FileSearchStoreListResponse
    {
        [JsonPropertyName("fileSearchStores")]
        public List<FileSearchStoreResponse>
            FileSearchStores
        { get; set; } = [];
    }

    private class FileSearchStoreResponse
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }
    }

    private class GeminiOperationResponse
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("done")]
        public bool Done { get; set; }

        [JsonPropertyName("error")]
        public GeminiOperationError? Error { get; set; }
    }

    private class GeminiOperationError
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}