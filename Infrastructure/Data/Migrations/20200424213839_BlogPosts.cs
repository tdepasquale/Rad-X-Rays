using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class BlogPosts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AppUserId = table.Column<string>(nullable: true),
                    OwnerUsername = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    CoverImageUrl = table.Column<string>(nullable: true),
                    IsSubmitted = table.Column<bool>(nullable: false),
                    IsPosted = table.Column<bool>(nullable: false),
                    DatePosted = table.Column<DateTime>(nullable: true),
                    Feedback = table.Column<string>(nullable: true),
                    PostType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    BlogId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogSections_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogSections_BlogId",
                table: "BlogSections",
                column: "BlogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogSections");

            migrationBuilder.DropTable(
                name: "Blogs");
        }
    }
}
