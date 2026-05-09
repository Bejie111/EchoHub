using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EchoHub.Migrations
{
    /// <inheritdoc />
    public partial class AddingAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "EwasteItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "EwasteItems");
        }
    }
}
