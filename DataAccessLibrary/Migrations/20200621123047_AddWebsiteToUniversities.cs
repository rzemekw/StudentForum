using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentForum.DataAccess.Migrations
{
    public partial class AddWebsiteToUniversities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Universities",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Website",
                table: "Universities");
        }
    }
}
