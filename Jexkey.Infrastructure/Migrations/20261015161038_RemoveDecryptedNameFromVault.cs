using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jexkey.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDecryptedNameFromVault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DecryptedName",
                table: "Vaults");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DecryptedName",
                table: "Vaults",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
