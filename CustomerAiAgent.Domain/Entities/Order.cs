using CustomerAiAgent.Domain.Enums;

namespace CustomerAiAgent.Domain.Entities;

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }

    public OrderStatus Status { get; set; }

    public Customer? Customer { get; set; }

    public ICollection<OrderItem> Items { get; set; }
        = new List<OrderItem>();
}