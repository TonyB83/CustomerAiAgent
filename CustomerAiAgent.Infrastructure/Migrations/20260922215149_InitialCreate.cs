using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CustomerAiAgent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Address", "Email", "FirstName", "LastName" },
                values: new object[,]
                {
                    { 1, "12 rue de Paris, 75001 Paris", "jean.dupont@example.com", "Jean", "Dupont" },
                    { 2, "25 avenue Victor Hugo, 75016 Paris", "sophie.martin@example.com", "Sophie", "Martin" },
                    { 3, "8 rue Nationale, 69001 Lyon", "thomas.bernard@example.com", "Thomas", "Bernard" },
                    { 4, "15 rue de la République, 69002 Lyon", "claire.petit@example.com", "Claire", "Petit" },
                    { 5, "42 boulevard Haussmann, 75009 Paris", "nicolas.robert@example.com", "Nicolas", "Robert" },
                    { 6, "18 rue de Lille, 59000 Lille", "julie.richard@example.com", "Julie", "Richard" },
                    { 7, "7 rue des Écoles, 33000 Bordeaux", "alexandre.moreau@example.com", "Alexandre", "Moreau" },
                    { 8, "31 avenue Jean Jaurès, 31000 Toulouse", "camille.simon@example.com", "Camille", "Simon" },
                    { 9, "22 rue Nationale, 44000 Nantes", "laurent.michel@example.com", "Laurent", "Michel" },
                    { 10, "5 rue Victor Hugo, 67000 Strasbourg", "emma.leroy@example.com", "Emma", "Leroy" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "IsActive", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { 1, "Ordinateur portable professionnel 15 pouces", true, "Ordinateur portable Dell", 899.99m, 25 },
                    { 2, "Écran professionnel QHD 27 pouces", true, "Écran Dell 27 pouces", 349.99m, 18 },
                    { 3, "Clavier mécanique USB avec rétroéclairage", true, "Clavier mécanique", 89.99m, 50 },
                    { 4, "Souris ergonomique sans fil", true, "Souris sans fil", 49.99m, 75 },
                    { 5, "Casque audio Bluetooth avec réduction de bruit", true, "Casque Bluetooth", 129.99m, 30 },
                    { 6, "Station d'accueil USB-C multi-écrans", true, "Station d'accueil USB-C", 159.99m, 22 },
                    { 7, "Disque SSD externe USB 3.2 de 1 To", true, "SSD externe 1 To", 119.99m, 40 },
                    { 8, "Webcam Full HD pour visioconférence", true, "Webcam Full HD", 69.99m, 35 },
                    { 9, "Sacoche professionnelle pour ordinateur 15 pouces", true, "Sacoche ordinateur", 59.99m, 45 },
                    { 10, "Chargeur rapide USB-C 100W", true, "Chargeur USB-C 100W", 79.99m, 60 }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "Amount", "CustomerId", "OrderDate", "Status" },
                values: new object[,]
                {
                    { 1, 149.99m, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Delivered" },
                    { 2, 299.90m, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shipped" },
                    { 3, 89.50m, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cancelled" },
                    { 4, 459.00m, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pending" },
                    { 5, 79.90m, 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Delivered" },
                    { 6, 199.99m, 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Confirmed" },
                    { 7, 349.00m, 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pending" },
                    { 8, 120.00m, 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cancelled" },
                    { 9, 580.50m, 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Delivered" },
                    { 10, 59.90m, 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shipped" },
                    { 11, 129.90m, 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Delivered" },
                    { 12, 249.00m, 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Delivered" },
                    { 13, 399.99m, 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pending" },
                    { 14, 89.99m, 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Confirmed" },
                    { 15, 199.00m, 5, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pending" },
                    { 16, 450.00m, 5, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cancelled" },
                    { 17, 75.50m, 6, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Delivered" },
                    { 18, 299.99m, 6, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shipped" },
                    { 19, 159.90m, 6, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Confirmed" },
                    { 20, 99.99m, 8, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cancelled" },
                    { 21, 249.90m, 8, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cancelled" },
                    { 22, 599.00m, 8, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Delivered" },
                    { 23, 129.00m, 8, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pending" },
                    { 24, 349.99m, 9, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Confirmed" },
                    { 25, 799.00m, 9, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shipped" },
                    { 26, 129.90m, 9, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Delivered" },
                    { 27, 69.99m, 10, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pending" },
                    { 28, 299.99m, 10, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Delivered" }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "OrderId", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, 5, 1, 129.99m },
                    { 2, 2, 2, 1, 349.99m },
                    { 3, 3, 3, 1, 89.99m },
                    { 4, 4, 1, 1, 899.99m },
                    { 5, 5, 4, 1, 49.99m },
                    { 6, 6, 6, 1, 159.99m },
                    { 7, 7, 7, 2, 119.99m },
                    { 8, 8, 8, 1, 69.99m },
                    { 9, 9, 1, 1, 899.99m },
                    { 10, 10, 9, 1, 59.99m },
                    { 11, 11, 5, 1, 129.99m },
                    { 12, 12, 2, 1, 349.99m },
                    { 13, 13, 1, 1, 899.99m },
                    { 14, 14, 3, 1, 89.99m },
                    { 15, 15, 10, 2, 79.99m },
                    { 16, 16, 6, 1, 159.99m },
                    { 17, 17, 8, 1, 69.99m },
                    { 18, 18, 7, 2, 119.99m },
                    { 19, 19, 4, 1, 49.99m },
                    { 20, 20, 3, 1, 89.99m },
                    { 21, 21, 5, 2, 129.99m },
                    { 22, 22, 1, 1, 899.99m },
                    { 23, 23, 9, 1, 59.99m },
                    { 24, 24, 2, 1, 349.99m },
                    { 25, 25, 1, 1, 899.99m },
                    { 26, 26, 7, 1, 119.99m },
                    { 27, 27, 10, 1, 79.99m },
                    { 28, 28, 6, 1, 159.99m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
