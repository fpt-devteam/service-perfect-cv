using System;
using Microsoft.EntityFrameworkCore.Migrations;
using ServicePerfectCV.Domain.ValueObjects;

#nullable disable

namespace ServicePerfectCV.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSectionScoreResultTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Summaries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Summaries",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Summaries",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "Jobs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "Jobs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "JobDescriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "JobDescriptions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "JobDescriptions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "DeviceTokens",
                type: "timestamptz",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Contacts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Contacts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Contacts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SectionScoreResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CVId = table.Column<Guid>(type: "uuid", nullable: false),
                    SectionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    JdHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SectionContentHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SectionScore = table.Column<SectionScore>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionScoreResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectionScoreResults_CVs_CVId",
                        column: x => x.CVId,
                        principalTable: "CVs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Summaries_DeletedAt",
                table: "Summaries",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_DeletedAt",
                table: "Jobs",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "IX_JobDescriptions_DeletedAt",
                table: "JobDescriptions",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_DeletedAt",
                table: "DeviceTokens",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_DeletedAt",
                table: "Contacts",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SectionScoreResults_CVId",
                table: "SectionScoreResults",
                column: "CVId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionScoreResults_CVId_SectionType",
                table: "SectionScoreResults",
                columns: new[] { "CVId", "SectionType" });

            migrationBuilder.CreateIndex(
                name: "IX_SectionScoreResults_CVId_SectionType_Hashes",
                table: "SectionScoreResults",
                columns: new[] { "CVId", "SectionType", "JdHash", "SectionContentHash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SectionScoreResults_DeletedAt",
                table: "SectionScoreResults",
                column: "DeletedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SectionScoreResults");

            migrationBuilder.DropIndex(
                name: "IX_Summaries_DeletedAt",
                table: "Summaries");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_DeletedAt",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_JobDescriptions_DeletedAt",
                table: "JobDescriptions");

            migrationBuilder.DropIndex(
                name: "IX_DeviceTokens_DeletedAt",
                table: "DeviceTokens");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_DeletedAt",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DeviceTokens");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Contacts");
        }
    }
}
