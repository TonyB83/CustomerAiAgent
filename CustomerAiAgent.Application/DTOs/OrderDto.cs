namespace CustomerAiAgent.Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = string.Empty;
}