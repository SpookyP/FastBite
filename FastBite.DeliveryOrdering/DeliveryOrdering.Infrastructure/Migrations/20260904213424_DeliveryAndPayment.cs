using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryOrdering.Infrastructure.Migrations
{
    /// <inheritdoc />
<<<<<<<< HEAD:FastBite.DeliveryOrdering/DeliveryOrdering.Infrastructure/Migrations/20260907154453_DeliveryOrdering.cs
    public partial class DeliveryOrdering : Migration
========
    public partial class DeliveryAndPayment : Migration
>>>>>>>> main:FastBite.DeliveryOrdering/DeliveryOrdering.Infrastructure/Migrations/20260904213424_DeliveryAndPayment.cs
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
<<<<<<<< HEAD:FastBite.DeliveryOrdering/DeliveryOrdering.Infrastructure/Migrations/20260907154453_DeliveryOrdering.cs
========
                    OrderType = table.Column<int>(type: "int", nullable: false),
>>>>>>>> main:FastBite.DeliveryOrdering/DeliveryOrdering.Infrastructure/Migrations/20260904213424_DeliveryAndPayment.cs
                    NomeCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactoTelefonico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Morada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoPostal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetodoPagamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
<<<<<<<< HEAD:FastBite.DeliveryOrdering/DeliveryOrdering.Infrastructure/Migrations/20260907154453_DeliveryOrdering.cs
                    TaxaEntrega = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
========
                    TaxaEntrega = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
>>>>>>>> main:FastBite.DeliveryOrdering/DeliveryOrdering.Infrastructure/Migrations/20260904213424_DeliveryAndPayment.cs
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    DescricaoItem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AcompanhamentoId = table.Column<int>(type: "int", nullable: true),
                    AcompanhamentoNome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BebidaId = table.Column<int>(type: "int", nullable: true),
                    BebidaNome = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
