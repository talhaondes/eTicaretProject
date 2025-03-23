using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eTicaretData.Migrations
{
    public partial class mg5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AdSoyad",
                table: "Orders",
                newName: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Orders",
                newName: "AdSoyad");
        }
    }
}
