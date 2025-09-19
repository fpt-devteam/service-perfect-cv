using Microsoft.EntityFrameworkCore.Migrations;
using ServicePerfectCV.Domain.ValueObjects;

#nullable disable

namespace ServicePerfectCV.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateJsonbForCVContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullContent",
                table: "CVs");

            migrationBuilder.AddColumn<CVContent>(
                name: "Content",
                table: "CVs",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "CVs");

            migrationBuilder.AddColumn<string>(
                name: "FullContent",
                table: "CVs",
                type: "text",
                nullable: true);
        }
    }
}