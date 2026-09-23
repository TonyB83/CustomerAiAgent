using CustomerAiAgent.Domain.Entities;
using CustomerAiAgent.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CustomerAiAgent.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        AppDbContext context)
    {
        // Création de la base si elle n'existe pas
        await context.Database.EnsureCreatedAsync();

        // Si des clients existent déjà, on ne recrée pas les données.
        if (await context.Customers.AnyAsync())
        {
            return;
        }

        // =========================================================
        // CLIENTS
        // =========================================================

        var customers = new List<Customer>
        {
            new Customer
            {
                Id = 1,
                FirstName = "Jean",
                LastName = "Dupont",
                Email = "jean.dupont@example.com",
                Address = "12 rue de Paris, 75001 Paris"
            },

            new Customer
            {
                Id = 2,
                FirstName = "Sophie",
                LastName = "Martin",
                Email = "sophie.martin@example.com",
                Address = "25 avenue Victor Hugo, 75016 Paris"
            },

            new Customer
            {
                Id = 3,
                FirstName = "Thomas",
                LastName = "Bernard",
                Email = "thomas.bernard@example.com",
                Address = "8 rue Nationale, 69001 Lyon"
            },

            new Customer
            {
                Id = 4,
                FirstName = "Claire",
                LastName = "Petit",
                Email = "claire.petit@example.com",
                Address = "15 rue de la République, 69002 Lyon"
            },

            new Customer
            {
                Id = 5,
                FirstName = "Nicolas",
                LastName = "Robert",
                Email = "nicolas.robert@example.com",
                Address = "42 boulevard Haussmann, 75009 Paris"
            },

            new Customer
            {
                Id = 6,
                FirstName = "Julie",
                LastName = "Richard",
                Email = "julie.richard@example.com",
                Address = "18 rue de Lille, 59000 Lille"
            },

            new Customer
            {
                Id = 7,
                FirstName = "Alexandre",
                LastName = "Moreau",
                Email = "alexandre.moreau@example.com",
                Address = "7 rue des Écoles, 33000 Bordeaux"
            },

            new Customer
            {
                Id = 8,
                FirstName = "Camille",
                LastName = "Simon",
                Email = "camille.simon@example.com",
                Address = "31 avenue Jean Jaurès, 31000 Toulouse"
            },

            new Customer
            {
                Id = 9,
                FirstName = "Laurent",
                LastName = "Michel",
                Email = "laurent.michel@example.com",
                Address = "22 rue Nationale, 44000 Nantes"
            },

            new Customer
            {
                Id = 10,
                FirstName = "Emma",
                LastName = "Leroy",
                Email = "emma.leroy@example.com",
                Address = "5 rue Victor Hugo, 67000 Strasbourg"
            }
        };

        await context.Customers.AddRangeAsync(customers);

        // =========================================================
        // COMMANDES
        // =========================================================

        var orders = new List<Order>
        {
            // =====================================================
            // CLIENT 1 - Jean Dupont
            // =====================================================

            new Order
            {
                Id = 1,
                CustomerId = 1,
                Amount = 149.99m,
                Status = OrderStatus.Delivered
            },

            new Order
            {
                Id = 2,
                CustomerId = 1,
                Amount = 299.90m,
                Status = OrderStatus.Shipped
            },

            new Order
            {
                Id = 3,
                CustomerId = 1,
                Amount = 89.50m,
                Status = OrderStatus.Cancelled
            },

            new Order
            {
                Id = 4,
                CustomerId = 1,
                Amount = 459.00m,
                Status = OrderStatus.Pending
            },

            // =====================================================
            // CLIENT 2 - Sophie Martin
            // =====================================================

            new Order
            {
                Id = 5,
                CustomerId = 2,
                Amount = 79.90m,
                Status = OrderStatus.Delivered
            },

            new Order
            {
                Id = 6,
                CustomerId = 2,
                Amount = 199.99m,
                Status = OrderStatus.Confirmed
            },

            new Order
            {
                Id = 7,
                CustomerId = 2,
                Amount = 349.00m,
                Status = OrderStatus.Pending
            },

            // =====================================================
            // CLIENT 3 - Thomas Bernard
            // =====================================================

            new Order
            {
                Id = 8,
                CustomerId = 3,
                Amount = 120.00m,
                Status = OrderStatus.Cancelled
            },

            new Order
            {
                Id = 9,
                CustomerId = 3,
                Amount = 580.50m,
                Status = OrderStatus.Delivered
            },

            // =====================================================
            // CLIENT 4 - Claire Petit
            // =====================================================

            new Order
            {
                Id = 10,
                CustomerId = 4,
                Amount = 59.90m,
                Status = OrderStatus.Shipped
            },

            new Order
            {
                Id = 11,
                CustomerId = 4,
                Amount = 129.90m,
                Status = OrderStatus.Delivered
            },

            new Order
            {
                Id = 12,
                CustomerId = 4,
                Amount = 249.00m,
                Status = OrderStatus.Delivered
            },

            new Order
            {
                Id = 13,
                CustomerId = 4,
                Amount = 399.99m,
                Status = OrderStatus.Pending
            },

            new Order
            {
                Id = 14,
                CustomerId = 4,
                Amount = 89.99m,
                Status = OrderStatus.Confirmed
            },

            // =====================================================
            // CLIENT 5 - Nicolas Robert
            // =====================================================

            new Order
            {
                Id = 15,
                CustomerId = 5,
                Amount = 199.00m,
                Status = OrderStatus.Pending
            },

            new Order
            {
                Id = 16,
                CustomerId = 5,
                Amount = 450.00m,
                Status = OrderStatus.Cancelled
            },

            // =====================================================
            // CLIENT 6 - Julie Richard
            // =====================================================

            new Order
            {
                Id = 17,
                CustomerId = 6,
                Amount = 75.50m,
                Status = OrderStatus.Delivered
            },

            new Order
            {
                Id = 18,
                CustomerId = 6,
                Amount = 299.99m,
                Status = OrderStatus.Shipped
            },

            new Order
            {
                Id = 19,
                CustomerId = 6,
                Amount = 159.90m,
                Status = OrderStatus.Confirmed
            },

            // =====================================================
            // CLIENT 7 - Alexandre Moreau
            // Aucun commande volontairement
            // =====================================================

            // =====================================================
            // CLIENT 8 - Camille Simon
            // =====================================================

            new Order
            {
                Id = 20,
                CustomerId = 8,
                Amount = 99.99m,
                Status = OrderStatus.Cancelled
            },

            new Order
            {
                Id = 21,
                CustomerId = 8,
                Amount = 249.90m,
                Status = OrderStatus.Cancelled
            },

            new Order
            {
                Id = 22,
                CustomerId = 8,
                Amount = 599.00m,
                Status = OrderStatus.Delivered
            },

            new Order
            {
                Id = 23,
                CustomerId = 8,
                Amount = 129.00m,
                Status = OrderStatus.Pending
            },

            // =====================================================
            // CLIENT 9 - Laurent Michel
            // =====================================================

            new Order
            {
                Id = 24,
                CustomerId = 9,
                Amount = 349.99m,
                Status = OrderStatus.Confirmed
            },

            new Order
            {
                Id = 25,
                CustomerId = 9,
                Amount = 799.00m,
                Status = OrderStatus.Shipped
            },

            new Order
            {
                Id = 26,
                CustomerId = 9,
                Amount = 129.90m,
                Status = OrderStatus.Delivered
            },

            // =====================================================
            // CLIENT 10 - Emma Leroy
            // =====================================================

            new Order
            {
                Id = 27,
                CustomerId = 10,
                Amount = 69.99m,
                Status = OrderStatus.Pending
            },

            new Order
            {
                Id = 28,
                CustomerId = 10,
                Amount = 299.99m,
                Status = OrderStatus.Delivered
            }
        };

        await context.Orders.AddRangeAsync(orders);

        await context.SaveChangesAsync();
    }
}