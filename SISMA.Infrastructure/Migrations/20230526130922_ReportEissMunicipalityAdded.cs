using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SISMA.Infrastructure.Migrations
{
    public partial class ReportEissMunicipalityAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "report_eispp_municipality",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    municipality_id = table.Column<int>(type: "integer", nullable: false),
                    city_code = table.Column<string>(type: "text", nullable: true),
                    report_data_id = table.Column<long>(type: "bigint", nullable: false),
                    catalog_code_id = table.Column<int>(type: "integer", nullable: false),
                    count_value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_report_eispp_municipality", x => x.id);
                    table.ForeignKey(
                        name: "fk_report_eispp_municipality_ek_munincipality_municipality_id",
                        column: x => x.municipality_id,
                        principalTable: "ek_munincipality",
                        principalColumn: "municipality_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_report_eispp_municipality_nom_catalog_code_catalog_code_id",
                        column: x => x.catalog_code_id,
                        principalTable: "nom_catalog_code",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_report_eispp_municipality_report_data_report_data_id",
                        column: x => x.report_data_id,
                        principalTable: "report_data",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_report_eispp_municipality_catalog_code_id",
                table: "report_eispp_municipality",
                column: "catalog_code_id");

            migrationBuilder.CreateIndex(
                name: "ix_report_eispp_municipality_municipality_id",
                table: "report_eispp_municipality",
                column: "municipality_id");

            migrationBuilder.CreateIndex(
                name: "ix_report_eispp_municipality_report_data_id",
                table: "report_eispp_municipality",
                column: "report_data_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "report_eispp_municipality");
        }
    }
}
