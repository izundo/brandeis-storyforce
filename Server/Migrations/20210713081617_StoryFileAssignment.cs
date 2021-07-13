using Microsoft.EntityFrameworkCore.Migrations;

namespace StoryForce.Server.Migrations
{
    public partial class StoryFileAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoryFileAssignment_StoryFiles_StoryFileId",
                table: "StoryFileAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryFiles_StoryFileAssignment_StoryFileAssignmentId",
                table: "StoryFiles");

            migrationBuilder.DropIndex(
                name: "IX_StoryFiles_StoryFileAssignmentId",
                table: "StoryFiles");

            migrationBuilder.DropColumn(
                name: "StoryFileAssignmentId",
                table: "StoryFiles");

            migrationBuilder.AlterColumn<int>(
                name: "StoryFileId",
                table: "StoryFileAssignment",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "StoryFileAssignment",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "AssignedToId",
                table: "StoryFileAssignment",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryFileAssignment_AssignedToId",
                table: "StoryFileAssignment",
                column: "AssignedToId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoryFileAssignment_AspNetUsers_AssignedToId",
                table: "StoryFileAssignment",
                column: "AssignedToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryFileAssignment_StoryFiles_StoryFileId",
                table: "StoryFileAssignment",
                column: "StoryFileId",
                principalTable: "StoryFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoryFileAssignment_AspNetUsers_AssignedToId",
                table: "StoryFileAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryFileAssignment_StoryFiles_StoryFileId",
                table: "StoryFileAssignment");

            migrationBuilder.DropIndex(
                name: "IX_StoryFileAssignment_AssignedToId",
                table: "StoryFileAssignment");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "StoryFileAssignment");

            migrationBuilder.AddColumn<int>(
                name: "StoryFileAssignmentId",
                table: "StoryFiles",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StoryFileId",
                table: "StoryFileAssignment",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "StoryFileAssignment",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryFiles_StoryFileAssignmentId",
                table: "StoryFiles",
                column: "StoryFileAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoryFileAssignment_StoryFiles_StoryFileId",
                table: "StoryFileAssignment",
                column: "StoryFileId",
                principalTable: "StoryFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryFiles_StoryFileAssignment_StoryFileAssignmentId",
                table: "StoryFiles",
                column: "StoryFileAssignmentId",
                principalTable: "StoryFileAssignment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
