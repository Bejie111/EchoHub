using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EchoHub.Migrations
{
    /// <inheritdoc />
    public partial class NewCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CollectionDate",
                table: "Collections",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectionDate",
                table: "Collections");
        }
    }
}
