using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicePerfectCV.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCertification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educations_Degree_DegreeId",
                table: "Educations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Degree",
                table: "Degree");

            migrationBuilder.DropColumn(
                name: "YearObtained",
                table: "Certifications");

            migrationBuilder.RenameTable(
                name: "Degree",
                newName: "Degrees");

            migrationBuilder.RenameColumn(
                name: "Relevance",
                table: "Certifications",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Issuer",
                table: "Certifications",
                newName: "Organization");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Certifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Certifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "IssuedDate",
                table: "Certifications",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "Certifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Certifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Degrees",
                table: "Degrees",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Certifications_OrganizationId",
                table: "Certifications",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certifications_Organizations_OrganizationId",
                table: "Certifications",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_Degrees_DegreeId",
                table: "Educations",
                column: "DegreeId",
                principalTable: "Degrees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certifications_Organizations_OrganizationId",
                table: "Certifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_Degrees_DegreeId",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Certifications_OrganizationId",
                table: "Certifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Degrees",
                table: "Degrees");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Certifications");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Certifications");

            migrationBuilder.DropColumn(
                name: "IssuedDate",
                table: "Certifications");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Certifications");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Certifications");

            migrationBuilder.RenameTable(
                name: "Degrees",
                newName: "Degree");

            migrationBuilder.RenameColumn(
                name: "Organization",
                table: "Certifications",
                newName: "Issuer");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Certifications",
                newName: "Relevance");

            migrationBuilder.AddColumn<int>(
                name: "YearObtained",
                table: "Certifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Degree",
                table: "Degree",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_Degree_DegreeId",
                table: "Educations",
                column: "DegreeId",
                principalTable: "Degree",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
