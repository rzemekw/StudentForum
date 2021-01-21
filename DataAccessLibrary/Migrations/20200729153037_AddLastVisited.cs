using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentForum.DataAccess.Migrations
{
    public partial class AddLastVisited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastVisited",
                table: "UserGroups",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "UserTopicDates",
                columns: table => new
                {
                    TopicId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    LastVisited = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTopicDates", x => new { x.TopicId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserTopicDates_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTopicDates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTopicDates_UserId",
                table: "UserTopicDates",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTopicDates");

            migrationBuilder.DropColumn(
                name: "LastVisited",
                table: "UserGroups");
        }
    }
}
