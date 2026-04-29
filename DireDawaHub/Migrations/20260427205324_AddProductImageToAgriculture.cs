using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DireDawaHub.Migrations
{
    /// <inheritdoc />
    public partial class AddProductImageToAgriculture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductImagePath",
                table: "AgricultureMarkets",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductImagePath",
                table: "AgricultureMarkets");
        }
    }
}
