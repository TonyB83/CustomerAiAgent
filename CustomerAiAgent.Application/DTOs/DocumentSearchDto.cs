namespace CustomerAiAgent.Application.DTOs;

public class DocumentSearchDto
{
    public string Answer { get; set; } = string.Empty;

    public List<DocumentCitationDto> Citations { get; set; } = [];
}

public class DocumentCitationDto
{
    public string FileName { get; set; } = string.Empty;

    public int? PageNumber { get; set; }

    public string Source { get; set; } = string.Empty;
}