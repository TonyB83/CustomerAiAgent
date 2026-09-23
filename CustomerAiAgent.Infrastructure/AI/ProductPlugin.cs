using System.ComponentModel;
using CustomerAiAgent.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;

namespace CustomerAiAgent.Infrastructure.AI;

public class ProductPlugin
{
    private readonly AppDbContext _dbContext;

    public ProductPlugin(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // ============================================================
    // GET PRODUCT
    // ============================================================

    [KernelFunction("get_product")]
    [Description(
        "Récupère les informations d'un produit à partir de son identifiant.")]
    public async Task<object?> GetProduct(
        int productId,
        CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .FirstOrDefaultAsync(
                p => p.Id == productId,
                cancellationToken);

        if (product == null)
        {
            return new
            {
                Found = false,
                Message = $"Le produit {productId} n'existe pas."
            };
        }

        return new
        {
            Found = true,
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.IsActive
        };
    }

    // ============================================================
    // SEARCH PRODUCTS
    // ============================================================

    [KernelFunction("search_products")]
    [Description(
        "Recherche des produits par nom ou description. " +
        "Retourne uniquement les produits actifs.")]
    public async Task<object> SearchProducts(
        string search,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return new
            {
                Count = 0,
                Products = Array.Empty<object>()
            };
        }

        search = search.Trim();

        var products = await _dbContext.Products
            .AsNoTracking()
            .Where(p =>
                p.IsActive &&
                (p.Name.Contains(search) ||
                 p.Description.Contains(search)))
            .OrderBy(p => p.Name)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Stock
            })
            .Take(20)
            .ToListAsync(cancellationToken);

        return new
        {
            Count = products.Count,
            Products = products
        };
    }

    // ============================================================
    // CHECK STOCK
    // ============================================================

    [KernelFunction("check_product_stock")]
    [Description(
        "Vérifie le stock disponible d'un produit à partir de son identifiant.")]
    public async Task<object> CheckProductStock(
        int productId,
        CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Stock
            })
            .FirstOrDefaultAsync(
                p => p.Id == productId,
                cancellationToken);

        if (product == null)
        {
            return new
            {
                Found = false,
                Message = $"Le produit {productId} n'existe pas."
            };
        }

        return new
        {
            Found = true,
            product.Id,
            product.Name,
            product.Stock,
            Available = product.Stock > 0
        };
    }

    // ============================================================
    // LIST PRODUCTS
    // ============================================================

    [KernelFunction("list_products")]
    [Description(
        "Retourne la liste des produits actifs disponibles.")]
    public async Task<object> ListProducts(
        CancellationToken cancellationToken = default)
    {
        var products = await _dbContext.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Stock
            })
            .ToListAsync(cancellationToken);

        return new
        {
            Count = products.Count,
            Products = products
        };
    }
}