using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DockerProject.Migrations
{
    /// <inheritdoc />
    public partial class ProjectSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AISummaryOverAll",
                table: "Projects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AiSummaryMembers",
                table: "Projects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AiSummaryProblemsIdentifyInComments",
                table: "Projects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AiSummaryTasks",
                table: "Projects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "SummaryRealizedAt",
                table: "Projects",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AISummaryOverAll",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "AiSummaryMembers",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "AiSummaryProblemsIdentifyInComments",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "AiSummaryTasks",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "SummaryRealizedAt",
                table: "Projects");
        }
    }
}
