using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrowMate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Plants",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                comment: "Путь к изображению растения");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Plants");
        }
    }
}
