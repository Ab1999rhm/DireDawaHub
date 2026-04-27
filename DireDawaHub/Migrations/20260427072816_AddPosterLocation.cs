using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DireDawaHub.Migrations
{
    /// <inheritdoc />
    public partial class AddPosterLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "CommunityPosters",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "CommunityPosters");
        }
    }
}
