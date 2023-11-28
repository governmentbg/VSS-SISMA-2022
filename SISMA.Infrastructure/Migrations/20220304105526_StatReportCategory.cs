using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SISMA.Infrastructure.Migrations
{
    public partial class StatReportCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "report_category_id",
                table: "nom_stat_report",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "nom_stat_report_category",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    integration_id = table.Column<int>(type: "integer", nullable: false),
                    order_number = table.Column<int>(type: "integer", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    label = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    date_start = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_end = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_nom_stat_report_category", x => x.id);
                    table.ForeignKey(
                        name: "fk_nom_stat_report_category_nom_integration_integration_id",
                        column: x => x.integration_id,
                        principalTable: "nom_integration",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_nom_stat_report_report_category_id",
                table: "nom_stat_report",
                column: "report_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_nom_stat_report_category_integration_id",
                table: "nom_stat_report_category",
                column: "integration_id");

            migrationBuilder.AddForeignKey(
                name: "fk_nom_stat_report_nom_stat_report_category_report_category_id",
                table: "nom_stat_report",
                column: "report_category_id",
                principalTable: "nom_stat_report_category",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_nom_stat_report_nom_stat_report_category_report_category_id",
                table: "nom_stat_report");

            migrationBuilder.DropTable(
                name: "nom_stat_report_category");

            migrationBuilder.DropIndex(
                name: "ix_nom_stat_report_report_category_id",
                table: "nom_stat_report");

            migrationBuilder.DropColumn(
                name: "report_category_id",
                table: "nom_stat_report");
        }
    }
}
