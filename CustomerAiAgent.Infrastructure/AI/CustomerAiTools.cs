using CustomerAiAgent.Application.Services;
using CustomerAiAgent.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CustomerAiAgent.Infrastructure.AI;

public class CustomerAiTools
{
    private readonly CustomerService _customerService;
    private readonly ILogger<CustomerAiTools> _logger;

    public CustomerAiTools(
        CustomerService customerService,
        ILogger<CustomerAiTools> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    public async Task<object> GetCustomer(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "AI Tool Call: get_customer. CustomerId={CustomerId}",
            customerId);

        if (customerId <= 0)
        {
            _logger.LogWarning(
                "AI Tool Validation failed: customerId={CustomerId}",
                customerId);

            return new
            {
                success = false,
                message = "L'identifiant client doit être supérieur à 0."
            };
        }

        try
        {
            var customer =
                await _customerService.GetCustomerAsync(
                    customerId,
                    cancellationToken);

            if (customer is null)
            {
                _logger.LogInformation(
                    "AI Tool get_customer: customer {CustomerId} not found",
                    customerId);

                return new
                {
                    success = false,
                    found = false,
                    message =
                        $"Le client {customerId} n'existe pas."
                };
            }

            _logger.LogInformation(
                "AI Tool get_customer succeeded. CustomerId={CustomerId}",
                customerId);

            return new
            {
                success = true,
                found = true,
                customer.Id,
                customer.FirstName,
                customer.LastName,
                customer.Email
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "AI Tool get_customer failed. CustomerId={CustomerId}",
                customerId);

            return new
            {
                success = false,
                message =
                    "Une erreur est survenue lors de la récupération du client."
            };
        }
    }

    public async Task<object> SearchCustomers(
        string search,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "AI Tool Call: search_customers. Search={Search}",
            search);

        if (string.IsNullOrWhiteSpace(search))
        {
            return new
            {
                success = false,
                message = "Le critère de recherche est obligatoire."
            };
        }

        search = search.Trim();

        if (search.Length < 2)
        {
            return new
            {
                success = false,
                message =
                    "Le critère de recherche doit contenir au moins 2 caractères."
            };
        }

        if (search.Length > 100)
        {
            return new
            {
                success = false,
                message =
                    "Le critère de recherche ne peut pas dépasser 100 caractères."
            };
        }

        try
        {
            var customers =
                await _customerService.SearchCustomersAsync(
                    search,
                    cancellationToken);

            _logger.LogInformation(
                "AI Tool search_customers succeeded. Search={Search}, Count={Count}",
                search,
                customers.Count);

            return new
            {
                success = true,
                search,
                count = customers.Count,
                customers = customers.Select(c => new
                {
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    c.Email
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "AI Tool search_customers failed. Search={Search}",
                search);

            return new
            {
                success = false,
                message =
                    "Une erreur est survenue lors de la recherche."
            };
        }
    }

    public async Task<object> GetOrders(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "AI Tool Call: get_orders. CustomerId={CustomerId}",
            customerId);

        if (customerId <= 0)
        {
            return new
            {
                success = false,
                message =
                    "L'identifiant client doit être supérieur à 0."
            };
        }

        try
        {
            var orders =
                await _customerService.GetOrdersAsync(
                    customerId,
                    cancellationToken);

            _logger.LogInformation(
                "AI Tool get_orders succeeded. CustomerId={CustomerId}, Count={Count}",
                customerId,
                orders.Count);

            return new
            {
                success = true,
                customerId,
                count = orders.Count,

                orders = orders.Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.Amount,
                    status = o.Status.ToString()
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "AI Tool get_orders failed. CustomerId={CustomerId}",
                customerId);

            return new
            {
                success = false,
                message =
                    "Une erreur est survenue lors de la récupération des commandes."
            };
        }
    }

    public async Task<object> GetOrder(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "AI Tool Call: get_order. OrderId={OrderId}",
            orderId);

        if (orderId <= 0)
        {
            return new
            {
                success = false,
                message =
                    "L'identifiant de commande doit être supérieur à 0."
            };
        }

        try
        {
            var order =
                await _customerService.GetOrderAsync(
                    orderId,
                    cancellationToken);

            if (order is null)
            {
                return new
                {
                    success = false,
                    found = false,
                    message =
                        $"La commande {orderId} n'existe pas."
                };
            }

            _logger.LogInformation(
                "AI Tool get_order succeeded. OrderId={OrderId}",
                orderId);

            return new
            {
                success = true,
                found = true,

                order.Id,
                order.CustomerId,
                order.OrderDate,
                order.Amount,
                status = order.Status.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "AI Tool get_order failed. OrderId={OrderId}",
                orderId);

            return new
            {
                success = false,
                message =
                    "Une erreur est survenue lors de la récupération de la commande."
            };
        }
    }

    public async Task<object> GetCustomerTotal(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "AI Tool Call: get_customer_total. CustomerId={CustomerId}",
            customerId);

        if (customerId <= 0)
        {
            return new
            {
                success = false,
                message =
                    "L'identifiant client doit être supérieur à 0."
            };
        }

        try
        {
            var total =
                await _customerService.GetCustomerTotalAmountAsync(
                    customerId,
                    cancellationToken);

            _logger.LogInformation(
                "AI Tool get_customer_total succeeded. CustomerId={CustomerId}, Total={Total}",
                customerId,
                total);

            return new
            {
                success = true,
                customerId,
                totalAmount = total
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "AI Tool get_customer_total failed. CustomerId={CustomerId}",
                customerId);

            return new
            {
                success = false,
                message =
                    "Une erreur est survenue lors du calcul du montant total."
            };
        }
    }

    public async Task<object> GetOrdersByStatus(
        int customerId,
        string status,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "AI Tool Call: get_orders_by_status. CustomerId={CustomerId}, Status={Status}",
            customerId,
            status);

        if (customerId <= 0)
        {
            return new
            {
                success = false,
                message =
                    "L'identifiant client doit être supérieur à 0."
            };
        }

        if (string.IsNullOrWhiteSpace(status))
        {
            return new
            {
                success = false,
                message =
                    "Le statut de commande est obligatoire."
            };
        }

        if (!Enum.TryParse<OrderStatus>(
                status.Trim(),
                ignoreCase: true,
                out var orderStatus))
        {
            _logger.LogWarning(
                "AI Tool Validation failed: invalid order status {Status}",
                status);

            return new
            {
                success = false,
                message =
                    $"Le statut '{status}' est invalide.",
                allowedStatuses = Enum
                    .GetNames<OrderStatus>()
            };
        }

        try
        {
            var orders =
                await _customerService
                    .GetOrdersByStatusAsync(
                        customerId,
                        orderStatus,
                        cancellationToken);

            _logger.LogInformation(
                "AI Tool get_orders_by_status succeeded. CustomerId={CustomerId}, Status={Status}, Count={Count}",
                customerId,
                orderStatus,
                orders.Count);

            return new
            {
                success = true,
                customerId,
                status = orderStatus.ToString(),
                count = orders.Count,

                orders = orders.Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.Amount,
                    status = o.Status.ToString()
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "AI Tool get_orders_by_status failed. CustomerId={CustomerId}, Status={Status}",
                customerId,
                orderStatus);

            return new
            {
                success = false,
                message =
                    "Une erreur est survenue lors de la récupération des commandes."
            };
        }
    }
}