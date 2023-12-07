using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyList_backend.Migrations
{
    public partial class mylistUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "MyLists",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MyLists_UserId",
                table: "MyLists",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MyLists_AspNetUsers_UserId",
                table: "MyLists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyLists_AspNetUsers_UserId",
                table: "MyLists");

            migrationBuilder.DropIndex(
                name: "IX_MyLists_UserId",
                table: "MyLists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MyLists");
        }
    }
}
