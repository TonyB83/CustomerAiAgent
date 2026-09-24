using CustomerAiAgent.Api;
using CustomerAiAgent.Application.Interfaces;
using CustomerAiAgent.Application.Services;
using CustomerAiAgent.Infrastructure.AI;
using CustomerAiAgent.Infrastructure.AI.Documents;
using CustomerAiAgent.Infrastructure.Documents;
using CustomerAiAgent.Infrastructure.Persistence;
using CustomerAiAgent.Infrastructure.Repositories;
using Google.GenAI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

// ============================================================
// DATABASE
// ============================================================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================================
// HTTP CLIENT
// ============================================================

builder.Services.AddHttpClient();

// ============================================================
// AI PROVIDER
// ============================================================

var providerString = builder.Configuration["AI:Provider"];

if (string.IsNullOrWhiteSpace(providerString))
{
    throw new InvalidOperationException(
        "AI:Provider est obligatoire. Valeurs possibles : OpenAI ou Gemini.");
}

if (!Enum.TryParse<AiProvider>(
        providerString,
        ignoreCase: true,
        out var aiProvider))
{
    throw new InvalidOperationException(
        $"AI:Provider '{providerString}' est invalide. " +
        "Valeurs possibles : OpenAI ou Gemini.");
}

// ============================================================
// DOCUMENT PROVIDER
// ============================================================

var documentProviderString =
    builder.Configuration["Documents:Provider"];

if (string.IsNullOrWhiteSpace(documentProviderString))
{
    throw new InvalidOperationException(
        "Documents:Provider est obligatoire. " +
        "Valeur possible : Gemini.");
}

if (!Enum.TryParse<DocumentProvider>(
        documentProviderString,
        ignoreCase: true,
        out var documentProvider))
{
    throw new InvalidOperationException(
        $"Documents:Provider '{documentProviderString}' est invalide.");
}

// ============================================================
// AI CONFIGURATION
// ============================================================

switch (aiProvider)
{
    // ========================================================
    // OPENAI
    // ========================================================

    case AiProvider.OpenAI:
        {
            var apiKey =
                builder.Configuration["OpenAI:ApiKey"];

            var model =
                builder.Configuration["OpenAI:Model"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "OpenAI:ApiKey est obligatoire.");
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                throw new InvalidOperationException(
                    "OpenAI:Model est obligatoire.");
            }

            // Semantic Kernel utilisera directement
            // le connecteur OpenAI.
            builder.Services.AddOpenAIChatCompletion(
                modelId: model,
                apiKey: apiKey);

            break;
        }

    // ========================================================
    // GEMINI
    // ========================================================

    case AiProvider.Gemini:
        {
            var apiKey =
                builder.Configuration["Gemini:ApiKey"];

            var model =
                builder.Configuration["Gemini:Model"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "Gemini:ApiKey est obligatoire.");
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                throw new InvalidOperationException(
                    "Gemini:Model est obligatoire.");
            }

            var geminiClient =
                new Google.GenAI.Client(apiKey: apiKey);

            builder.Services.AddSingleton(geminiClient);

            // Gemini -> IChatClient
            var chatClient =
                geminiClient
                    .AsIChatClient(model)
                    .AsBuilder()
                    .UseFunctionInvocation()
                    .Build();

            builder.Services.AddSingleton<IChatClient>(
                chatClient);

            // IChatClient -> Semantic Kernel
            builder.Services.AddScoped<
                IChatCompletionService>(sp =>
                {
                    var client =
                    sp.GetRequiredService<IChatClient>();

                    return client.AsChatCompletionService(sp);
                });

            break;
        }

    default:
        throw new InvalidOperationException(
            $"Provider AI non supporté : {aiProvider}");
}

// ============================================================
// DOCUMENT / RAG PROVIDER
// ============================================================

switch (documentProvider)
{
    case DocumentProvider.Gemini:
        {
            // Gemini File Search
            builder.Services.AddSingleton<
                IDocumentSearchService,
                GeminiFileSearchService>();

            break;
        }

    default:
        throw new InvalidOperationException(
            $"Document provider non supporté : {documentProvider}");
}

// ============================================================
// REPOSITORIES
// ============================================================

builder.Services.AddScoped<
    ICustomerRepository,
    CustomerRepository>();

// ============================================================
// APPLICATION SERVICES
// ============================================================

builder.Services.AddScoped<CustomerService>();

// ============================================================
// AI TOOLS
// ============================================================

builder.Services.AddScoped<CustomerAiTools>();

// ============================================================
// KERNEL PLUGINS
// ============================================================

builder.Services.AddScoped<CustomerPlugin>();
builder.Services.AddScoped<OrderPlugin>();
builder.Services.AddScoped<ProductPlugin>();

// DocumentPlugin utilise IDocumentSearchService.
// Il ne connaît donc pas Gemini directement.
builder.Services.AddScoped<DocumentPlugin>();

// ============================================================
// SEMANTIC KERNEL
// ============================================================

builder.Services.AddTransient<Kernel>(sp =>
{
    var kernel = new Kernel(sp);

    // --------------------------------------------------------
    // CUSTOMER
    // --------------------------------------------------------

    var customerPlugin =
        sp.GetRequiredService<CustomerPlugin>();

    kernel.Plugins.AddFromObject(
        customerPlugin,
        "Customer");

    // --------------------------------------------------------
    // ORDER
    // --------------------------------------------------------

    var orderPlugin =
        sp.GetRequiredService<OrderPlugin>();

    kernel.Plugins.AddFromObject(
        orderPlugin,
        "Order");

    // --------------------------------------------------------
    // PRODUCT
    // --------------------------------------------------------

    var productPlugin =
        sp.GetRequiredService<ProductPlugin>();

    kernel.Plugins.AddFromObject(
        productPlugin,
        "Product");

    // --------------------------------------------------------
    // DOCUMENT
    // --------------------------------------------------------

    var documentPlugin =
        sp.GetRequiredService<DocumentPlugin>();

    kernel.Plugins.AddFromObject(
        documentPlugin,
        "Document");

    return kernel;
});

// ============================================================
// AI SERVICE
// ============================================================

builder.Services.AddScoped<IAiService, ChatAgentService>();

// ============================================================
// MVC / API / RAZOR
// ============================================================

builder.Services.AddControllers();

builder.Services.AddRazorPages();

// ============================================================
// APPLICATION
// ============================================================

var app = builder.Build();

// ============================================================
// HTTP PIPELINE
// ============================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Si ton projet utilise JWT, conserver ces deux middlewares.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

app.Run();