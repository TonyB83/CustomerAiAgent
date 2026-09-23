using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace CustomerAiAgent.Infrastructure.AI;

public class OrderPlugin
{
    private readonly CustomerAiTools _tools;

    public OrderPlugin(CustomerAiTools tools)
    {
        _tools = tools;
    }

    [KernelFunction("get_orders")]
    [Description(
        "Récupère toutes les commandes d'un client.")]
    public async Task<object> GetOrders(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        return await _tools.GetOrders(
            customerId,
            cancellationToken);
    }

    [KernelFunction("get_order")]
    [Description(
        "Récupère une commande précise à partir de son identifiant.")]
    public async Task<object> GetOrder(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        return await _tools.GetOrder(
            orderId,
            cancellationToken);
    }

    [KernelFunction("get_customer_total")]
    [Description(
        "Calcule le montant total des commandes d'un client.")]
    public async Task<object> GetCustomerTotal(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        return await _tools.GetCustomerTotal(
            customerId,
            cancellationToken);
    }

    [KernelFunction("get_orders_by_status")]
    [Description(
        "Récupère les commandes d'un client selon leur statut.")]
    public async Task<object> GetOrdersByStatus(
        int customerId,
        string status,
        CancellationToken cancellationToken = default)
    {
        return await _tools.GetOrdersByStatus(
            customerId,
            status,
            cancellationToken);
    }
}