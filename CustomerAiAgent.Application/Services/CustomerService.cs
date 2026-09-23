using CustomerAiAgent.Application.Interfaces;
using CustomerAiAgent.Domain.Entities;
using CustomerAiAgent.Domain.Enums;

namespace CustomerAiAgent.Application.Services;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(
        ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Customer?> GetCustomerAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        return await _customerRepository.GetCustomerAsync(
            customerId,
            cancellationToken);
    }

    public async Task<List<Order>> GetOrdersAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        return await _customerRepository.GetOrdersAsync(
            customerId,
            cancellationToken);
    }

    public async Task<Order?> GetOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        return await _customerRepository.GetOrderAsync(
            orderId,
            cancellationToken);
    }

    public async Task<decimal> GetCustomerTotalAmountAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        var orders =
            await _customerRepository.GetOrdersAsync(
                customerId,
                cancellationToken);

        return orders.Sum(o => o.Amount);
    }

    public async Task<List<Customer>> SearchCustomersAsync(
        string search,
        CancellationToken cancellationToken = default)
    {
        return await _customerRepository.SearchCustomersAsync(
            search,
            cancellationToken);
    }

    public async Task<List<Order>> GetOrdersByStatusAsync(
        int customerId,
        OrderStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _customerRepository.GetOrdersByStatusAsync(
            customerId,
            status,
            cancellationToken);
    }
}