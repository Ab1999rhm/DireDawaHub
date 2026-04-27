using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DireDawaHub.Migrations
{
    /// <inheritdoc />
    public partial class AddPosterContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "CommunityPosters",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "CommunityPosters");
        }
    }
}
