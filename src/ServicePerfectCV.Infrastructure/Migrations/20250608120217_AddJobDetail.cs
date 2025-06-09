using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace ServicePerfectCV.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certifications_CVs_CVSId",
                table: "Certifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_CVs_CVSId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_CVs_Contacts_ContactId",
                table: "CVs");

            migrationBuilder.DropForeignKey(
                name: "FK_CVs_Summaries_SummaryId",
                table: "CVs");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_CVs_CVSId",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_CVs_CVSId",
                table: "Experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_CVs_CVSId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_CVs_CVSId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_Summaries_CVs_CvId",
                table: "Summaries");

            migrationBuilder.DropIndex(
                name: "IX_Summaries_CvId",
                table: "Summaries");

            migrationBuilder.DropIndex(
                name: "IX_CVs_ContactId",
                table: "CVs");

            migrationBuilder.DropIndex(
                name: "IX_CVs_SummaryId",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "CVSId",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "SummaryId",
                table: "CVs");

            migrationBuilder.RenameColumn(
                name: "CvId",
                table: "Summaries",
                newName: "CVId");

            migrationBuilder.RenameColumn(
                name: "CVSId",
                table: "Skills",
                newName: "CVId");

            migrationBuilder.RenameIndex(
                name: "IX_Skills_CVSId",
                table: "Skills",
                newName: "IX_Skills_CVId");

            migrationBuilder.RenameColumn(
                name: "CVSId",
                table: "Projects",
                newName: "CVId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_CVSId",
                table: "Projects",
                newName: "IX_Projects_CVId");

            migrationBuilder.RenameColumn(
                name: "CVSId",
                table: "Experiences",
                newName: "CVId");

            migrationBuilder.RenameIndex(
                name: "IX_Experiences_CVSId",
                table: "Experiences",
                newName: "IX_Experiences_CVId");

            migrationBuilder.RenameColumn(
                name: "CVSId",
                table: "Educations",
                newName: "CVId");

            migrationBuilder.RenameIndex(
                name: "IX_Educations_CVSId",
                table: "Educations",
                newName: "IX_Educations_CVId");

            migrationBuilder.RenameColumn(
                name: "CVSId",
                table: "Contacts",
                newName: "CVId");

            migrationBuilder.RenameIndex(
                name: "IX_Contacts_CVSId",
                table: "Contacts",
                newName: "IX_Contacts_CVId");

            migrationBuilder.RenameColumn(
                name: "CVSId",
                table: "Certifications",
                newName: "CVId");

            migrationBuilder.RenameIndex(
                name: "IX_Certifications_CVSId",
                table: "Certifications",
                newName: "IX_Certifications_CVId");

            migrationBuilder.AlterColumn<Guid>(
                name: "CVId",
                table: "Summaries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobCompany",
                table: "CVs",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobDescription",
                table: "CVs",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "CVs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Summaries_CVId",
                table: "Summaries",
                column: "CVId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Certifications_CVs_CVId",
                table: "Certifications",
                column: "CVId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_CVs_CVId",
                table: "Contacts",
                column: "CVId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_CVs_CVId",
                table: "Educations",
                column: "CVId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_CVs_CVId",
                table: "Experiences",
                column: "CVId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_CVs_CVId",
                table: "Projects",
                column: "CVId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_CVs_CVId",
                table: "Skills",
                column: "CVId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Summaries_CVs_CVId",
                table: "Summaries",
                column: "CVId",
                principalTable: "CVs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certifications_CVs_CVId",
                table: "Certifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_CVs_CVId",
                table: "Contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_CVs_CVId",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_CVs_CVId",
                table: "Experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_CVs_CVId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_CVs_CVId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_Summaries_CVs_CVId",
                table: "Summaries");

            migrationBuilder.DropIndex(
                name: "IX_Summaries_CVId",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "JobCompany",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "JobDescription",
                table: "CVs");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "CVs");

            migrationBuilder.RenameColumn(
                name: "CVId",
                table: "Summaries",
                newName: "CvId");

            migrationBuilder.RenameColumn(
                name: "CVId",
                table: "Skills",
                newName: "CVSId");

            migrationBuilder.RenameIndex(
                name: "IX_Skills_CVId",
                table: "Skills",
                newName: "IX_Skills_CVSId");

            migrationBuilder.RenameColumn(
                name: "CVId",
                table: "Projects",
                newName: "CVSId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_CVId",
                table: "Projects",
                newName: "IX_Projects_CVSId");

            migrationBuilder.RenameColumn(
                name: "CVId",
                table: "Experiences",
                newName: "CVSId");

            migrationBuilder.RenameIndex(
                name: "IX_Experiences_CVId",
                table: "Experiences",
                newName: "IX_Experiences_CVSId");

            migrationBuilder.RenameColumn(
                name: "CVId",
                table: "Educations",
                newName: "CVSId");

            migrationBuilder.RenameIndex(
                name: "IX_Educations_CVId",
                table: "Educations",
                newName: "IX_Educations_CVSId");

            migrationBuilder.RenameColumn(
                name: "CVId",
                table: "Contacts",
                newName: "CVSId");

            migrationBuilder.RenameIndex(
                name: "IX_Contacts_CVId",
                table: "Contacts",
                newName: "IX_Contacts_CVSId");

            migrationBuilder.RenameColumn(
                name: "CVId",
                table: "Certifications",
                newName: "CVSId");

            migrationBuilder.RenameIndex(
                name: "IX_Certifications_CVId",
                table: "Certifications",
                newName: "IX_Certifications_CVSId");

            migrationBuilder.AlterColumn<Guid>(
                name: "CvId",
                table: "Summaries",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CVSId",
                table: "Summaries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ContactId",
                table: "CVs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SummaryId",
                table: "CVs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Summaries_CvId",
                table: "Summaries",
                column: "CvId");

            migrationBuilder.CreateIndex(
                name: "IX_CVs_ContactId",
                table: "CVs",
                column: "ContactId",
                unique: true,
                filter: "[ContactId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CVs_SummaryId",
                table: "CVs",
                column: "SummaryId",
                unique: true,
                filter: "[SummaryId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Certifications_CVs_CVSId",
                table: "Certifications",
                column: "CVSId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_CVs_CVSId",
                table: "Contacts",
                column: "CVSId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CVs_Contacts_ContactId",
                table: "CVs",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CVs_Summaries_SummaryId",
                table: "CVs",
                column: "SummaryId",
                principalTable: "Summaries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_CVs_CVSId",
                table: "Educations",
                column: "CVSId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_CVs_CVSId",
                table: "Experiences",
                column: "CVSId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_CVs_CVSId",
                table: "Projects",
                column: "CVSId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_CVs_CVSId",
                table: "Skills",
                column: "CVSId",
                principalTable: "CVs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Summaries_CVs_CvId",
                table: "Summaries",
                column: "CvId",
                principalTable: "CVs",
                principalColumn: "Id");
        }
    }
}