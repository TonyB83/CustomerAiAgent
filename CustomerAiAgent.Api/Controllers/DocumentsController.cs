using CustomerAiAgent.Application.Interfaces;
using CustomerAiAgent.Infrastructure.AI.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerAiAgent.Api.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentSearchService _documentSearchService;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(
        IDocumentSearchService documentSearchService,
        ILogger<DocumentsController> logger)
    {
        _documentSearchService = documentSearchService;
        _logger = logger;

    }

    // RECHERCHE DOCUMENTAIRE
    // ============================================================

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] DocumentQuestionRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest(new
            {
                message = "La question est obligatoire."
            });
        }

        try
        {
            var result =
                await _documentSearchService.SearchAsync(
                    request.Question,
                    cancellationToken);

            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "La recherche documentaire a été annulée.");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erreur lors de la recherche documentaire.");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = "Une erreur est survenue lors de la recherche documentaire."
                });
        }
    }


    // ==========================================
    // TEST 2
    // Recherche documentaire sans génération IA
    // ==========================================

    //[HttpPost("ask")]
    //public IActionResult Ask(
    //    [FromBody] DocumentQuestionRequest request)
    //{
    //    if (string.IsNullOrWhiteSpace(request.Question))
    //    {
    //        return BadRequest(new
    //        {
    //            message = "La question est obligatoire."
    //        });
    //    }

    //    var results =
    //        _documentSearchService.Search(
    //            request.Question,
    //            5);

    //    return Ok(new
    //    {
    //        question = request.Question,
    //        found = results.Count > 0,
    //        count = results.Count,
    //        documents = results
    //    });
    //}

    //[HttpPost("file-search")]
    //public async Task<IActionResult> FileSearch(
    //    [FromBody] DocumentQuestionRequest request,
    //    CancellationToken cancellationToken)
    //{
    //    if (string.IsNullOrWhiteSpace(request.Question))
    //    {
    //        return BadRequest(new
    //        {
    //            message = "La question est obligatoire."
    //        });
    //    }

    //    var result =
    //        await _fileSearchService.SearchAsync(
    //            request.Question,
    //            cancellationToken);

    //    return Ok(result);
    //}

    //[HttpGet("semantic-search")]
    //public async Task<IActionResult> SemanticSearch(
    //[FromQuery] string q,
    //CancellationToken cancellationToken)
    //{
    //    if (string.IsNullOrWhiteSpace(q))
    //    {
    //        return BadRequest(new
    //        {
    //            message = "Le paramètre q est obligatoire."
    //        });
    //    }

    //    var results =
    //        await _semanticDocumentSearchService.SearchAsync(
    //            q,
    //            5,
    //            cancellationToken);

    //    return Ok(new
    //    {
    //        query = q,
    //        count = results.Count,
    //        results
    //    });
    //}
}

public record DocumentQuestionRequest(
    string Question);