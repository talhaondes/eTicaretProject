using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eTicaretData.Migrations
{
    public partial class mg9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Adresses_AspNetUsers_UserId1",
                table: "Adresses");

            migrationBuilder.DropIndex(
                name: "IX_Adresses_UserId1",
                table: "Adresses");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Adresses");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Adresses",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Adresses_UserId",
                table: "Adresses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Adresses_AspNetUsers_UserId",
                table: "Adresses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Adresses_AspNetUsers_UserId",
                table: "Adresses");

            migrationBuilder.DropIndex(
                name: "IX_Adresses_UserId",
                table: "Adresses");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Adresses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Adresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Adresses_UserId1",
                table: "Adresses",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Adresses_AspNetUsers_UserId1",
                table: "Adresses",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
