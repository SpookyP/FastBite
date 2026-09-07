using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenuCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantidadeVendida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuantidadeVendidaHoje",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantidadeVendidaHoje",
                table: "Items");
        }
    }
}
