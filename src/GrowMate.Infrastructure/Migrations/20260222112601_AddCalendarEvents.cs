using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GrowMate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCalendarEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false, comment: "Идентификатор пользователя-владельца"),
                    Title = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false, comment: "Название события"),
                    Type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, comment: "Тип события"),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Дата и время события"),
                    Notes = table.Column<string>(type: "text", nullable: true, comment: "Описание события")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_UserId_ScheduledAt",
                table: "CalendarEvents",
                columns: new[] { "UserId", "ScheduledAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarEvents");
        }
    }
}
