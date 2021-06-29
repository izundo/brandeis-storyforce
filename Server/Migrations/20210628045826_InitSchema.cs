using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace StoryForce.Server.Migrations
{
    public partial class InitSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentCategoryId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_Category_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    ClassOfYear = table.Column<int>(type: "integer", nullable: true),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryStoryFile",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "integer", nullable: false),
                    StoryFilesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryStoryFile", x => new { x.CategoriesId, x.StoryFilesId });
                    table.ForeignKey(
                        name: "FK_CategoryStoryFile_Category_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Key = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: true),
                    Keywords = table.Column<List<string>>(type: "text[]", nullable: true),
                    SubmissionId = table.Column<int>(type: "integer", nullable: false),
                    ApprovedSubmissionId = table.Column<int>(type: "integer", nullable: true),
                    RejectedSubmissionId = table.Column<int>(type: "integer", nullable: true),
                    DownloadUrl = table.Column<string>(type: "text", nullable: true),
                    Class = table.Column<int>(type: "integer", nullable: true),
                    Size = table.Column<long>(type: "bigint", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SubmittedById = table.Column<int>(type: "integer", nullable: true),
                    RequestedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoryFiles_Persons_RequestedById",
                        column: x => x.RequestedById,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoryFiles_Persons_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoryFiles_Persons_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StoryFileId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_StoryFiles_StoryFileId",
                        column: x => x.StoryFileId,
                        principalTable: "StoryFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Category = table.Column<int>(type: "integer", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    StoryFileId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_StoryFiles_StoryFileId",
                        column: x => x.StoryFileId,
                        principalTable: "StoryFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    StoryFileId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_StoryFiles_StoryFileId",
                        column: x => x.StoryFileId,
                        principalTable: "StoryFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonStoryFile",
                columns: table => new
                {
                    FeaturedPeopleId = table.Column<int>(type: "integer", nullable: false),
                    FeaturedStoryFileId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonStoryFile", x => new { x.FeaturedPeopleId, x.FeaturedStoryFileId });
                    table.ForeignKey(
                        name: "FK_PersonStoryFile_Persons_FeaturedPeopleId",
                        column: x => x.FeaturedPeopleId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonStoryFile_StoryFiles_FeaturedStoryFileId",
                        column: x => x.FeaturedStoryFileId,
                        principalTable: "StoryFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryFileTag",
                columns: table => new
                {
                    StoryFilesId = table.Column<int>(type: "integer", nullable: false),
                    TagsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryFileTag", x => new { x.StoryFilesId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_StoryFileTag_StoryFiles_StoryFilesId",
                        column: x => x.StoryFilesId,
                        principalTable: "StoryFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryFileTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoryStoryFile",
                columns: table => new
                {
                    BelongsToId = table.Column<int>(type: "integer", nullable: false),
                    FilesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryStoryFile", x => new { x.BelongsToId, x.FilesId });
                    table.ForeignKey(
                        name: "FK_StoryStoryFile_Stories_BelongsToId",
                        column: x => x.BelongsToId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryStoryFile_StoryFiles_FilesId",
                        column: x => x.FilesId,
                        principalTable: "StoryFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubmittedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReviewedById = table.Column<int>(type: "integer", nullable: true),
                    ApprovedById = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    EventId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submissions_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Submissions_Persons_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Submissions_Persons_ReviewedById",
                        column: x => x.ReviewedById,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Submissions_Persons_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StoryFileId = table.Column<int>(type: "integer", nullable: true),
                    SubmissionId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditDetails_StoryFiles_StoryFileId",
                        column: x => x.StoryFileId,
                        principalTable: "StoryFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditDetails_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonSubmission",
                columns: table => new
                {
                    FeaturedPeopleId = table.Column<int>(type: "integer", nullable: false),
                    FeaturedSubmissionsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonSubmission", x => new { x.FeaturedPeopleId, x.FeaturedSubmissionsId });
                    table.ForeignKey(
                        name: "FK_PersonSubmission_Persons_FeaturedPeopleId",
                        column: x => x.FeaturedPeopleId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonSubmission_Submissions_FeaturedSubmissionsId",
                        column: x => x.FeaturedSubmissionsId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditDetails_StoryFileId",
                table: "AuditDetails",
                column: "StoryFileId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditDetails_SubmissionId",
                table: "AuditDetails",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_ParentCategoryId",
                table: "Category",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryStoryFile_StoryFilesId",
                table: "CategoryStoryFile",
                column: "StoryFilesId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_StoryFileId",
                table: "Comments",
                column: "StoryFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_StoryFileId",
                table: "Events",
                column: "StoryFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_StoryFileId",
                table: "Notes",
                column: "StoryFileId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonStoryFile_FeaturedStoryFileId",
                table: "PersonStoryFile",
                column: "FeaturedStoryFileId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonSubmission_FeaturedSubmissionsId",
                table: "PersonSubmission",
                column: "FeaturedSubmissionsId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryFiles_ApprovedSubmissionId",
                table: "StoryFiles",
                column: "ApprovedSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryFiles_EventId",
                table: "StoryFiles",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryFiles_RejectedSubmissionId",
                table: "StoryFiles",
                column: "RejectedSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryFiles_RequestedById",
                table: "StoryFiles",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_StoryFiles_SubmissionId",
                table: "StoryFiles",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryFiles_SubmittedById",
                table: "StoryFiles",
                column: "SubmittedById");

            migrationBuilder.CreateIndex(
                name: "IX_StoryFiles_UpdatedById",
                table: "StoryFiles",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StoryFileTag_TagsId",
                table: "StoryFileTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_StoryStoryFile_FilesId",
                table: "StoryStoryFile",
                column: "FilesId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ApprovedById",
                table: "Submissions",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_EventId",
                table: "Submissions",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ReviewedById",
                table: "Submissions",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_SubmittedById",
                table: "Submissions",
                column: "SubmittedById");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryStoryFile_StoryFiles_StoryFilesId",
                table: "CategoryStoryFile",
                column: "StoryFilesId",
                principalTable: "StoryFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryFiles_Events_EventId",
                table: "StoryFiles",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryFiles_Submissions_ApprovedSubmissionId",
                table: "StoryFiles",
                column: "ApprovedSubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryFiles_Submissions_RejectedSubmissionId",
                table: "StoryFiles",
                column: "RejectedSubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryFiles_Submissions_SubmissionId",
                table: "StoryFiles",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_StoryFiles_StoryFileId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "AuditDetails");

            migrationBuilder.DropTable(
                name: "CategoryStoryFile");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "PersonStoryFile");

            migrationBuilder.DropTable(
                name: "PersonSubmission");

            migrationBuilder.DropTable(
                name: "StoryFileTag");

            migrationBuilder.DropTable(
                name: "StoryStoryFile");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Stories");

            migrationBuilder.DropTable(
                name: "StoryFiles");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
