using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrowMate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantQuantityAndIsUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUnique",
                table: "Plants",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Является ли растение уникальным (true) или групповым (false)");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Plants",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                comment: "Количество растений (для групповых посадок)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUnique",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Plants");
        }
    }
}
