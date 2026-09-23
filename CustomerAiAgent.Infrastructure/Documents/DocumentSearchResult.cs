namespace CustomerAiAgent.Infrastructure.AI.Documents;

public class DocumentSearchResult
{
    public int DocumentId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public double Score { get; set; }
}