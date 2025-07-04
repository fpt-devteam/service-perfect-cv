using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePerfectCV.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCvTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AnalysisId",
                table: "CVs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VersionId",
                table: "CVs",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnalysisId",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "VersionId",
                table: "CVs");
        }
    }
}
