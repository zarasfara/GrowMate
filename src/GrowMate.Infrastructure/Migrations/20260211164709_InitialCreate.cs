using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GrowMate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GardenBeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Название грядки"),
                    Type = table.Column<string>(type: "text", nullable: false, comment: "Тип грядки"),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false, comment: "Идентификатор пользователя-владельца")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GardenBeds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlantTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Название культуры (шаблон)"),
                    Type = table.Column<string>(type: "text", nullable: false, comment: "Тип культуры"),
                    Description = table.Column<string>(type: "text", nullable: true, comment: "Описание культуры"),
                    WateringIntervalDays = table.Column<int>(type: "integer", nullable: true, comment: "Рекомендуемый интервал полива в днях"),
                    FirstFertilizingAfterDays = table.Column<int>(type: "integer", nullable: true, comment: "Через сколько дней после посадки рекомендована первая подкормка"),
                    FirstTreatmentAfterDays = table.Column<int>(type: "integer", nullable: true, comment: "Через сколько дней после посадки рекомендована первая обработка")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Название растения"),
                    Variety = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, comment: "Сорт"),
                    Description = table.Column<string>(type: "text", nullable: true, comment: "Описание / Заметки"),
                    PlantingDate = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "NOW()", comment: "Дата посадки"),
                    Type = table.Column<string>(type: "text", nullable: false, comment: "Тип растения"),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    PlantTemplateId = table.Column<int>(type: "integer", nullable: true, comment: "Идентификатор шаблона культуры"),
                    GardenBedId = table.Column<int>(type: "integer", nullable: false, comment: "Идентификатор грядки")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plants_GardenBeds_GardenBedId",
                        column: x => x.GardenBedId,
                        principalTable: "GardenBeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Plants_PlantTemplates_PlantTemplateId",
                        column: x => x.PlantTemplateId,
                        principalTable: "PlantTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "GardenTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "Задача"),
                    Type = table.Column<string>(type: "text", nullable: false, comment: "Тип задачи"),
                    DueDate = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "NOW()", comment: "Срок выполнения"),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "Выполнено"),
                    PlantId = table.Column<int>(type: "integer", nullable: false, comment: "Идентификатор растения")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GardenTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GardenTasks_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GardenTasks_IsCompleted_DueDate",
                table: "GardenTasks",
                columns: new[] { "IsCompleted", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_GardenTasks_PlantId",
                table: "GardenTasks",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_Plants_GardenBedId",
                table: "Plants",
                column: "GardenBedId");

            migrationBuilder.CreateIndex(
                name: "IX_Plants_PlantTemplateId",
                table: "Plants",
                column: "PlantTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GardenTasks");

            migrationBuilder.DropTable(
                name: "Plants");

            migrationBuilder.DropTable(
                name: "GardenBeds");

            migrationBuilder.DropTable(
                name: "PlantTemplates");
        }
    }
}
