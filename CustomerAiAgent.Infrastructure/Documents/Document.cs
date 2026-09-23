namespace CustomerAiAgent.Infrastructure.AI.Documents;

public class Document
{
    public int Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
}