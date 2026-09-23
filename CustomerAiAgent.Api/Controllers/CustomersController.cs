using CustomerAiAgent.Application.Services;
using CustomerAiAgent.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerAiAgent.Api.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomersController(
        CustomerService customerService)
    {
        _customerService = customerService;
    }


    // GET /api/customers/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCustomer(
        int id,
        CancellationToken cancellationToken)
    {
        var customer =
            await _customerService.GetCustomerAsync(
                id,
                cancellationToken);

        if (customer is null)
        {
            return NotFound(new
            {
                message = $"Client {id} introuvable."
            });
        }

        return Ok(customer);
    }


    // GET /api/customers/1/orders
    [HttpGet("{id:int}/orders")]
    public async Task<IActionResult> GetOrders(
        int id,
        CancellationToken cancellationToken)
    {
        var orders =
            await _customerService.GetOrdersAsync(
                id,
                cancellationToken);

        return Ok(orders);
    }


    // GET /api/customers/orders/1
    [HttpGet("orders/{orderId:int}")]
    public async Task<IActionResult> GetOrder(
        int orderId,
        CancellationToken cancellationToken)
    {
        var order =
            await _customerService.GetOrderAsync(
                orderId,
                cancellationToken);

        if (order is null)
        {
            return NotFound(new
            {
                message =
                    $"Commande {orderId} introuvable."
            });
        }

        return Ok(order);
    }


    // GET /api/customers/1/total
    [HttpGet("{id:int}/total")]
    public async Task<IActionResult> GetCustomerTotal(
        int id,
        CancellationToken cancellationToken)
    {
        var total =
            await _customerService.GetCustomerTotalAmountAsync(
                id,
                cancellationToken);

        return Ok(new
        {
            customerId = id,
            totalAmount = total
        });
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchCustomers(
    [FromQuery] string search,
    CancellationToken cancellationToken)
    {
        var customers =
            await _customerService.SearchCustomersAsync(
                search,
                cancellationToken);

        return Ok(
            customers.Select(c => new
            {
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email
            }));
    }

    [HttpGet("{id:int}/orders/status/{status}")]
    public async Task<IActionResult> GetOrdersByStatus(
    int id,
    string status,
    CancellationToken cancellationToken)
    {
        var orders =
            await _customerService.GetOrdersByStatusAsync(
                id,
                Enum.Parse<OrderStatus>(status),
                cancellationToken);

        return Ok(
            orders.Select(o => new
            {
                o.Id,
                o.OrderDate,
                o.Amount,
                o.Status
            }));
    }
}