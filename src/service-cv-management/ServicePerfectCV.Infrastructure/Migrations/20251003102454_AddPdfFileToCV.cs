using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePerfectCV.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPdfFileToCV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PdfContentType",
                table: "CVs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PdfFile",
                table: "CVs",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PdfFileName",
                table: "CVs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfContentType",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "PdfFile",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "PdfFileName",
                table: "CVs");
        }
    }
}
