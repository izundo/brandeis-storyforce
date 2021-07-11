using Microsoft.EntityFrameworkCore.Migrations;

namespace StoryForce.Server.Migrations
{
    public partial class AddNotesToSubmission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubmissionId",
                table: "Notes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notes_SubmissionId",
                table: "Notes",
                column: "SubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Submissions_SubmissionId",
                table: "Notes",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Submissions_SubmissionId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_SubmissionId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "SubmissionId",
                table: "Notes");
        }
    }
}
