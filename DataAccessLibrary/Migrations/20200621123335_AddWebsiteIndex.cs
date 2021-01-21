using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentForum.DataAccess.Migrations
{
    public partial class AddWebsiteIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Universities_Website",
                table: "Universities",
                column: "Website");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Universities_Website",
                table: "Universities");
        }
    }
}
