using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePerfectCV.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSkillTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Skills");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Skills",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skills_CategoryId",
                table: "Skills",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Categories_CategoryId",
                table: "Skills",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Categories_CategoryId",
                table: "Skills");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Skills_CategoryId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Skills");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Skills",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
