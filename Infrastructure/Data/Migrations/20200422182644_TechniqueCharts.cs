using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class TechniqueCharts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TechniqueCharts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AppUserId = table.Column<string>(nullable: true),
                    OwnerUsername = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechniqueCharts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Techniques",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BodyPart = table.Column<string>(nullable: true),
                    mAs = table.Column<float>(nullable: false),
                    kVp = table.Column<float>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    Index = table.Column<int>(nullable: false),
                    TechniqueChartId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Techniques", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Techniques_TechniqueCharts_TechniqueChartId",
                        column: x => x.TechniqueChartId,
                        principalTable: "TechniqueCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Techniques_TechniqueChartId",
                table: "Techniques",
                column: "TechniqueChartId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Techniques");

            migrationBuilder.DropTable(
                name: "TechniqueCharts");
        }
    }
}
