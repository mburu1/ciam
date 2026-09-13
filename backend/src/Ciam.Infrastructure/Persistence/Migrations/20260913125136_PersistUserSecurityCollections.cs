using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ciam.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PersistUserSecurityCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "authentication_methods",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "mfa_methods",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "roles",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "authentication_methods",
                table: "users");

            migrationBuilder.DropColumn(
                name: "mfa_methods",
                table: "users");

            migrationBuilder.DropColumn(
                name: "roles",
                table: "users");
        }
    }
}
