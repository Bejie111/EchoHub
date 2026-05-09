using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EchoHub.Migrations
{
    /// <inheritdoc />
    public partial class DropStaffId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
            name: "FK_Collections_Users_StaffId",
            table: "Collections");

            migrationBuilder.DropIndex(
                name: "IX_Collections_StaffId",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Collections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
            name: "StaffId",
            table: "Collections",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "IX_Collections_StaffId",
            table: "Collections",
            column: "StaffId");

        migrationBuilder.AddForeignKey(
            name: "FK_Collections_Users_StaffId",
            table: "Collections",
            column: "StaffId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
        }
    }
}
