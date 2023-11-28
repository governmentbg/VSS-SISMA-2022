using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SISMA.Infrastructure.Migrations
{
    public partial class UsersUICadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "full_name",
                table: "identity_users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "identity_users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "uic",
                table: "identity_users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "full_name",
                table: "identity_users");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "identity_users");

            migrationBuilder.DropColumn(
                name: "uic",
                table: "identity_users");
        }
    }
}
