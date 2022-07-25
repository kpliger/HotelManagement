using Microsoft.EntityFrameworkCore.Migrations;

namespace HotelManagement.Data.Migrations
{
    public partial class AddUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "creator",
                table: "Item");

            migrationBuilder.AddColumn<string>(
                name: "creatorId",
                table: "Item",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_creatorId",
                table: "Item",
                column: "creatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_AspNetUsers_creatorId",
                table: "Item",
                column: "creatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_AspNetUsers_creatorId",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_creatorId",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "creatorId",
                table: "Item");

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "Item",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
