using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SISMA.Infrastructure.Migrations
{
    public partial class CourtDistancesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "common_court_distance",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    court_id = table.Column<int>(type: "integer", nullable: false),
                    ek_ekatte_id = table.Column<int>(type: "integer", nullable: false),
                    distance = table.Column<decimal>(type: "numeric", nullable: false),
                    duration = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_common_court_distance", x => x.id);
                    table.ForeignKey(
                        name: "fk_common_court_distance_common_court_court_id",
                        column: x => x.court_id,
                        principalTable: "common_court",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_common_court_distance_ek_ekatte_ek_ekatte_id",
                        column: x => x.ek_ekatte_id,
                        principalTable: "ek_ekatte",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_common_court_distance_court_id",
                table: "common_court_distance",
                column: "court_id");

            migrationBuilder.CreateIndex(
                name: "ix_common_court_distance_ek_ekatte_id",
                table: "common_court_distance",
                column: "ek_ekatte_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "common_court_distance");
        }
    }
}
