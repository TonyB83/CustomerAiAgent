namespace CustomerAiAgent.Infrastructure.AI.Documents;

public class DocumentChunk
{
    public int Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public float[] Embedding { get; set; } = [];

    public int ChunkIndex { get; set; }
}