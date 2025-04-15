using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentsFeeSystem.Migrations
{
    /// <inheritdoc />
    public partial class sixToTwelveClassAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedToClass1",
                table: "FeeItems");

            migrationBuilder.DropColumn(
                name: "AssignedToClass2",
                table: "FeeItems");

            migrationBuilder.DropColumn(
                name: "AssignedToClass3",
                table: "FeeItems");

            migrationBuilder.DropColumn(
                name: "AssignedToClass4",
                table: "FeeItems");

            migrationBuilder.DropColumn(
                name: "AssignedToClass5",
                table: "FeeItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AssignedToClass1",
                table: "FeeItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AssignedToClass2",
                table: "FeeItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AssignedToClass3",
                table: "FeeItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AssignedToClass4",
                table: "FeeItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AssignedToClass5",
                table: "FeeItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
