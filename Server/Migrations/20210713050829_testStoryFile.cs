using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace StoryForce.Server.Migrations
{
    public partial class testStoryFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoryFileAssignmentId",
                table: "StoryFiles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoryFileAssignment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StoryFileId = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FileStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryFileAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryFileAssignment_StoryFiles_StoryFileId",
                        column: x => x.StoryFileId,
                        principalTable: "StoryFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoryFiles_StoryFileAssignmentId",
                table: "StoryFiles",
                column: "StoryFileAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryFileAssignment_StoryFileId",
                table: "StoryFileAssignment",
                column: "StoryFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoryFiles_StoryFileAssignment_StoryFileAssignmentId",
                table: "StoryFiles",
                column: "StoryFileAssignmentId",
                principalTable: "StoryFileAssignment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoryFiles_StoryFileAssignment_StoryFileAssignmentId",
                table: "StoryFiles");

            migrationBuilder.DropTable(
                name: "StoryFileAssignment");

            migrationBuilder.DropIndex(
                name: "IX_StoryFiles_StoryFileAssignmentId",
                table: "StoryFiles");

            migrationBuilder.DropColumn(
                name: "StoryFileAssignmentId",
                table: "StoryFiles");
        }
    }
}
