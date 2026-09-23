using CustomerAiAgent.Domain.Entities;
using CustomerAiAgent.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CustomerAiAgent.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =====================================================
        // CUSTOMER
        // =====================================================

        modelBuilder.Entity<Customer>()
            .HasKey(c => c.Id);

        // =====================================================
        // ORDER
        // =====================================================

        modelBuilder.Entity<Order>()
            .HasKey(o => o.Id);

        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Order>()
            .Property(o => o.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // =====================================================
        // PRODUCT
        // =====================================================

        modelBuilder.Entity<Product>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        // =====================================================
        // ORDER ITEM
        // =====================================================

        modelBuilder.Entity<OrderItem>()
            .HasKey(i => i.Id);

        modelBuilder.Entity<OrderItem>()
            .Property(i => i.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // =====================================================
        // SEED DATA
        // =====================================================

        SeedCustomers(modelBuilder);

        SeedProducts(modelBuilder);

        SeedOrders(modelBuilder);

        SeedOrderItems(modelBuilder);
    }

    // =========================================================
    // CUSTOMERS
    // =========================================================

    private static void SeedCustomers(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasData(

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
        );
    }

    // =========================================================
    // PRODUCTS
    // =========================================================

    private static void SeedProducts(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(

            new Product
            {
                Id = 1,
                Name = "Ordinateur portable Dell",
                Description = "Ordinateur portable professionnel 15 pouces",
                Price = 899.99m,
                Stock = 25,
                IsActive = true
            },

            new Product
            {
                Id = 2,
                Name = "Écran Dell 27 pouces",
                Description = "Écran professionnel QHD 27 pouces",
                Price = 349.99m,
                Stock = 18,
                IsActive = true
            },

            new Product
            {
                Id = 3,
                Name = "Clavier mécanique",
                Description = "Clavier mécanique USB avec rétroéclairage",
                Price = 89.99m,
                Stock = 50,
                IsActive = true
            },

            new Product
            {
                Id = 4,
                Name = "Souris sans fil",
                Description = "Souris ergonomique sans fil",
                Price = 49.99m,
                Stock = 75,
                IsActive = true
            },

            new Product
            {
                Id = 5,
                Name = "Casque Bluetooth",
                Description = "Casque audio Bluetooth avec réduction de bruit",
                Price = 129.99m,
                Stock = 30,
                IsActive = true
            },

            new Product
            {
                Id = 6,
                Name = "Station d'accueil USB-C",
                Description = "Station d'accueil USB-C multi-écrans",
                Price = 159.99m,
                Stock = 22,
                IsActive = true
            },

            new Product
            {
                Id = 7,
                Name = "SSD externe 1 To",
                Description = "Disque SSD externe USB 3.2 de 1 To",
                Price = 119.99m,
                Stock = 40,
                IsActive = true
            },

            new Product
            {
                Id = 8,
                Name = "Webcam Full HD",
                Description = "Webcam Full HD pour visioconférence",
                Price = 69.99m,
                Stock = 35,
                IsActive = true
            },

            new Product
            {
                Id = 9,
                Name = "Sacoche ordinateur",
                Description = "Sacoche professionnelle pour ordinateur 15 pouces",
                Price = 59.99m,
                Stock = 45,
                IsActive = true
            },

            new Product
            {
                Id = 10,
                Name = "Chargeur USB-C 100W",
                Description = "Chargeur rapide USB-C 100W",
                Price = 79.99m,
                Stock = 60,
                IsActive = true
            }
        );
    }

    // =========================================================
    // ORDERS
    // =========================================================

    private static void SeedOrders(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>().HasData(

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
        );
    }

    // =========================================================
    // ORDER ITEMS
    // =========================================================

    private static void SeedOrderItems(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderItem>().HasData(

            // Order 1
            new OrderItem
            {
                Id = 1,
                OrderId = 1,
                ProductId = 5,
                Quantity = 1,
                UnitPrice = 129.99m
            },

            // Order 2
            new OrderItem
            {
                Id = 2,
                OrderId = 2,
                ProductId = 2,
                Quantity = 1,
                UnitPrice = 349.99m
            },

            // Order 3
            new OrderItem
            {
                Id = 3,
                OrderId = 3,
                ProductId = 3,
                Quantity = 1,
                UnitPrice = 89.99m
            },

            // Order 4
            new OrderItem
            {
                Id = 4,
                OrderId = 4,
                ProductId = 1,
                Quantity = 1,
                UnitPrice = 899.99m
            },

            // Order 5
            new OrderItem
            {
                Id = 5,
                OrderId = 5,
                ProductId = 4,
                Quantity = 1,
                UnitPrice = 49.99m
            },

            // Order 6
            new OrderItem
            {
                Id = 6,
                OrderId = 6,
                ProductId = 6,
                Quantity = 1,
                UnitPrice = 159.99m
            },

            // Order 7
            new OrderItem
            {
                Id = 7,
                OrderId = 7,
                ProductId = 7,
                Quantity = 2,
                UnitPrice = 119.99m
            },

            // Order 8
            new OrderItem
            {
                Id = 8,
                OrderId = 8,
                ProductId = 8,
                Quantity = 1,
                UnitPrice = 69.99m
            },

            // Order 9
            new OrderItem
            {
                Id = 9,
                OrderId = 9,
                ProductId = 1,
                Quantity = 1,
                UnitPrice = 899.99m
            },

            // Order 10
            new OrderItem
            {
                Id = 10,
                OrderId = 10,
                ProductId = 9,
                Quantity = 1,
                UnitPrice = 59.99m
            },

            // Order 11
            new OrderItem
            {
                Id = 11,
                OrderId = 11,
                ProductId = 5,
                Quantity = 1,
                UnitPrice = 129.99m
            },

            // Order 12
            new OrderItem
            {
                Id = 12,
                OrderId = 12,
                ProductId = 2,
                Quantity = 1,
                UnitPrice = 349.99m
            },

            // Order 13
            new OrderItem
            {
                Id = 13,
                OrderId = 13,
                ProductId = 1,
                Quantity = 1,
                UnitPrice = 899.99m
            },

            // Order 14
            new OrderItem
            {
                Id = 14,
                OrderId = 14,
                ProductId = 3,
                Quantity = 1,
                UnitPrice = 89.99m
            },

            // Order 15
            new OrderItem
            {
                Id = 15,
                OrderId = 15,
                ProductId = 10,
                Quantity = 2,
                UnitPrice = 79.99m
            },

            // Order 16
            new OrderItem
            {
                Id = 16,
                OrderId = 16,
                ProductId = 6,
                Quantity = 1,
                UnitPrice = 159.99m
            },

            // Order 17
            new OrderItem
            {
                Id = 17,
                OrderId = 17,
                ProductId = 8,
                Quantity = 1,
                UnitPrice = 69.99m
            },

            // Order 18
            new OrderItem
            {
                Id = 18,
                OrderId = 18,
                ProductId = 7,
                Quantity = 2,
                UnitPrice = 119.99m
            },

            // Order 19
            new OrderItem
            {
                Id = 19,
                OrderId = 19,
                ProductId = 4,
                Quantity = 1,
                UnitPrice = 49.99m
            },

            // Order 20
            new OrderItem
            {
                Id = 20,
                OrderId = 20,
                ProductId = 3,
                Quantity = 1,
                UnitPrice = 89.99m
            },

            // Order 21
            new OrderItem
            {
                Id = 21,
                OrderId = 21,
                ProductId = 5,
                Quantity = 2,
                UnitPrice = 129.99m
            },

            // Order 22
            new OrderItem
            {
                Id = 22,
                OrderId = 22,
                ProductId = 1,
                Quantity = 1,
                UnitPrice = 899.99m
            },

            // Order 23
            new OrderItem
            {
                Id = 23,
                OrderId = 23,
                ProductId = 9,
                Quantity = 1,
                UnitPrice = 59.99m
            },

            // Order 24
            new OrderItem
            {
                Id = 24,
                OrderId = 24,
                ProductId = 2,
                Quantity = 1,
                UnitPrice = 349.99m
            },

            // Order 25
            new OrderItem
            {
                Id = 25,
                OrderId = 25,
                ProductId = 1,
                Quantity = 1,
                UnitPrice = 899.99m
            },

            // Order 26
            new OrderItem
            {
                Id = 26,
                OrderId = 26,
                ProductId = 7,
                Quantity = 1,
                UnitPrice = 119.99m
            },

            // Order 27
            new OrderItem
            {
                Id = 27,
                OrderId = 27,
                ProductId = 10,
                Quantity = 1,
                UnitPrice = 79.99m
            },

            // Order 28
            new OrderItem
            {
                Id = 28,
                OrderId = 28,
                ProductId = 6,
                Quantity = 1,
                UnitPrice = 159.99m
            }
        );
    }
}