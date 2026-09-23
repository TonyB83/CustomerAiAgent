using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace CustomerAiAgent.Infrastructure.AI;

public class CustomerPlugin
{
    private readonly CustomerAiTools _tools;

    public CustomerPlugin(CustomerAiTools tools)
    {
        _tools = tools;
    }

    [KernelFunction("get_customer")]
    [Description(
        "Récupère les informations générales d'un client à partir de son identifiant.")]
    public async Task<object> GetCustomer(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        return await _tools.GetCustomer(
            customerId,
            cancellationToken);
    }

    [KernelFunction("search_customers")]
    [Description(
        "Recherche des clients par prénom, nom ou adresse email.")]
    public async Task<object> SearchCustomers(
        string search,
        CancellationToken cancellationToken = default)
    {
        return await _tools.SearchCustomers(
            search,
            cancellationToken);
    }
}