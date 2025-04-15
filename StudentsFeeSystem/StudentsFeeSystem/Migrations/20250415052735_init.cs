using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentsFeeSystem.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeeItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    AssignedToMale = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssignedToFemale = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssignedToClass1 = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssignedToClass2 = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssignedToClass3 = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssignedToClass4 = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssignedToClass5 = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssignedToClass6 = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssignedToClass7 = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssignedToClass8 = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssignedToClass9 = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssignedToClass10 = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssignedToClass11 = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssignedToClass12 = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FathersName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Roll = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEdit = table.Column<bool>(type: "INTEGER", nullable: false),
                    Class = table.Column<int>(type: "INTEGER", nullable: false),
                    Fee = table.Column<decimal>(type: "TEXT", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HasPaid = table.Column<bool>(type: "INTEGER", nullable: false),
                    Gender = table.Column<int>(type: "INTEGER", nullable: false),
                    Department = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GenderFeeAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FeeItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Gender = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenderFeeAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenderFeeAssignments_FeeItems_FeeItemId",
                        column: x => x.FeeItemId,
                        principalTable: "FeeItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenderFeeAssignments_FeeItemId",
                table: "GenderFeeAssignments",
                column: "FeeItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenderFeeAssignments");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "FeeItems");
        }
    }
}
