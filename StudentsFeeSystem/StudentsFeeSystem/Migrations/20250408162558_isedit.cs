using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentsFeeSystem.Migrations
{
    /// <inheritdoc />
    public partial class isedit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEdit",
                table: "Students",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEdit",
                table: "Students");
        }
    }
}
