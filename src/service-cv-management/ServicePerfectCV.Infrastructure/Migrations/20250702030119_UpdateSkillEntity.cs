using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePerfectCV.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSkillEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ItemsJson",
                table: "Skills",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Skills",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Skills",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Skills",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Skills",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Skills");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Skills",
                newName: "ItemsJson");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Skills",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
