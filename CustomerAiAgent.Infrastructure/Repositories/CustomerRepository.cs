using CustomerAiAgent.Application.Interfaces;
using CustomerAiAgent.Domain.Entities;
using CustomerAiAgent.Domain.Enums;
using CustomerAiAgent.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerAiAgent.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetCustomerAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(
                c => c.Id == customerId,
                cancellationToken);
    }

    public async Task<List<Order>> GetOrdersAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .FirstOrDefaultAsync(
                o => o.Id == orderId,
                cancellationToken);
    }
    public async Task<List<Customer>> SearchCustomersAsync(
    string search,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return [];
        }

        search = search.Trim();

        return await _context.Customers
            .Where(c =>
                c.FirstName.Contains(search) ||
                c.LastName.Contains(search) ||
                c.Email.Contains(search))
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync(cancellationToken);
    }


    public async Task<List<Order>> GetOrdersByStatusAsync(
    int customerId,
    OrderStatus status,
    CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Where(o =>
                o.CustomerId == customerId &&
                o.Status == status)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }
}