using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISMA.Infrastructure.Migrations
{
    public partial class ApplicationRoleLabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "label",
                table: "identity_roles",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "label",
                table: "identity_roles");
        }
    }
}
