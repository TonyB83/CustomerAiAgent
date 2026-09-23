namespace CustomerAiAgent.Infrastructure.AI.Documents;

public class GeminiFileSearchResult
{
    public string Answer { get; set; } = string.Empty;

    public List<GeminiFileCitation> Citations { get; set; } = [];
}

public class GeminiFileCitation
{
    public string FileName { get; set; } = string.Empty;

    public int? PageNumber { get; set; }

    public string Source { get; set; } = string.Empty;
}