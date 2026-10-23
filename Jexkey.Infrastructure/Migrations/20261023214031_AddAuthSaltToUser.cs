using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jexkey.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthSaltToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "AuthSalt",
                table: "Users",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthSalt",
                table: "Users");
        }
    }
}
