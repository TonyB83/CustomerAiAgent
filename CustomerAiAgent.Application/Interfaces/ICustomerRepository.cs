using CustomerAiAgent.Domain.Entities;
using CustomerAiAgent.Domain.Enums;

namespace CustomerAiAgent.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetCustomerAsync(
        int customerId,
        CancellationToken cancellationToken = default);

    Task<List<Order>> GetOrdersAsync(
        int customerId,
        CancellationToken cancellationToken = default);

    Task<Order?> GetOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default);

    Task<List<Customer>> SearchCustomersAsync(
        string search,
        CancellationToken cancellationToken = default);

    Task<List<Order>> GetOrdersByStatusAsync(
        int customerId,
        OrderStatus status,
        CancellationToken cancellationToken = default);
}