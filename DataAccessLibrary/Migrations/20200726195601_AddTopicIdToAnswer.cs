using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentForum.DataAccess.Migrations
{
    public partial class AddTopicIdToAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachements_Answers_TopicEntryId",
                table: "Attachements");

            migrationBuilder.DropIndex(
                name: "IX_Attachements_TopicEntryId",
                table: "Attachements");

            migrationBuilder.DropColumn(
                name: "TopicEntryId",
                table: "Attachements");

            migrationBuilder.AddColumn<int>(
                name: "TopicAnswerId",
                table: "Attachements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TopicId",
                table: "Answers",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachements_TopicAnswerId",
                table: "Attachements",
                column: "TopicAnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachements_Answers_TopicAnswerId",
                table: "Attachements",
                column: "TopicAnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachements_Answers_TopicAnswerId",
                table: "Attachements");

            migrationBuilder.DropIndex(
                name: "IX_Attachements_TopicAnswerId",
                table: "Attachements");

            migrationBuilder.DropColumn(
                name: "TopicAnswerId",
                table: "Attachements");

            migrationBuilder.AddColumn<int>(
                name: "TopicEntryId",
                table: "Attachements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TopicId",
                table: "Answers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Attachements_TopicEntryId",
                table: "Attachements",
                column: "TopicEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachements_Answers_TopicEntryId",
                table: "Attachements",
                column: "TopicEntryId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
