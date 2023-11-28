using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SISMA.Infrastructure.Migrations
{
    public partial class LogOperAlter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "log_operations");

            migrationBuilder.CreateTable(
                name: "log_operation",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    operation = table.Column<string>(type: "text", nullable: true),
                    controller = table.Column<string>(type: "text", nullable: true),
                    action = table.Column<string>(type: "text", nullable: true),
                    object_id = table.Column<string>(type: "text", nullable: true),
                    user_wrt = table.Column<string>(type: "text", nullable: true),
                    date_wrt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    user_data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_log_operation", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "log_operation");

            migrationBuilder.CreateTable(
                name: "log_operations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    action = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    controller = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    master_key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    object_key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    operation_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    operation_type_id = table.Column<int>(type: "integer", nullable: false),
                    operation_user = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    operation_user_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    user_data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_log_operations", x => x.id);
                });
        }
    }
}
