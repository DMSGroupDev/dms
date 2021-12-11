using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace dms_backend_api.Data.Migrations
{
    public partial class UpdateStructs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dev");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUtc",
                schema: "dev",
                table: "applicationuser",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUtc",
                schema: "dev",
                table: "applicationrole",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                schema: "dev",
                table: "applicationrole",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOnUtc",
                schema: "dev",
                table: "applicationuser");

            migrationBuilder.DropColumn(
                name: "CreatedOnUtc",
                schema: "dev",
                table: "applicationrole");

            migrationBuilder.DropColumn(
                name: "Priority",
                schema: "dev",
                table: "applicationrole");

        }
    }
}
