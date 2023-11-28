using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISMA.Infrastructure.Migrations
{
    public partial class DistanceTypeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_nom_stat_report_nom_stat_report_category_report_category_id",
                table: "nom_stat_report");

            migrationBuilder.AlterColumn<int>(
                name: "report_category_id",
                table: "nom_stat_report",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "distance_type",
                table: "common_court_distance",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_nom_stat_report_nom_stat_report_category_report_category_id",
                table: "nom_stat_report",
                column: "report_category_id",
                principalTable: "nom_stat_report_category",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_nom_stat_report_nom_stat_report_category_report_category_id",
                table: "nom_stat_report");

            migrationBuilder.DropColumn(
                name: "distance_type",
                table: "common_court_distance");

            migrationBuilder.AlterColumn<int>(
                name: "report_category_id",
                table: "nom_stat_report",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "fk_nom_stat_report_nom_stat_report_category_report_category_id",
                table: "nom_stat_report",
                column: "report_category_id",
                principalTable: "nom_stat_report_category",
                principalColumn: "id");
        }
    }
}
