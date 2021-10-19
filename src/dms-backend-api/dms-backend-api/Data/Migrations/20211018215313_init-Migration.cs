using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace dms_backend_api.Data.Migrations
{
    public partial class initMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "applicationrole",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applicationrole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "applicationuser",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applicationuser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "identityroleclaim<guid>",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identityroleclaim<guid>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_identityroleclaim<guid>_applicationrole_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "public",
                        principalTable: "applicationrole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identityuserclaim<guid>",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identityuserclaim<guid>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_identityuserclaim<guid>_applicationuser_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "applicationuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identityuserlogin<guid>",
                schema: "public",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identityuserlogin<guid>", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_identityuserlogin<guid>_applicationuser_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "applicationuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identityuserrole<guid>",
                schema: "public",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identityuserrole<guid>", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_identityuserrole<guid>_applicationrole_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "public",
                        principalTable: "applicationrole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_identityuserrole<guid>_applicationuser_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "applicationuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identityusertoken<guid>",
                schema: "public",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identityusertoken<guid>", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_identityusertoken<guid>_applicationuser_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "applicationuser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "public",
                table: "applicationrole",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "public",
                table: "applicationuser",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "public",
                table: "applicationuser",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_identityroleclaim<guid>_RoleId",
                schema: "public",
                table: "identityroleclaim<guid>",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_identityuserclaim<guid>_UserId",
                schema: "public",
                table: "identityuserclaim<guid>",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_identityuserlogin<guid>_UserId",
                schema: "public",
                table: "identityuserlogin<guid>",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_identityuserrole<guid>_RoleId",
                schema: "public",
                table: "identityuserrole<guid>",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "identityroleclaim<guid>",
                schema: "public");

            migrationBuilder.DropTable(
                name: "identityuserclaim<guid>",
                schema: "public");

            migrationBuilder.DropTable(
                name: "identityuserlogin<guid>",
                schema: "public");

            migrationBuilder.DropTable(
                name: "identityuserrole<guid>",
                schema: "public");

            migrationBuilder.DropTable(
                name: "identityusertoken<guid>",
                schema: "public");

            migrationBuilder.DropTable(
                name: "applicationrole",
                schema: "public");

            migrationBuilder.DropTable(
                name: "applicationuser",
                schema: "public");
        }
    }
}
