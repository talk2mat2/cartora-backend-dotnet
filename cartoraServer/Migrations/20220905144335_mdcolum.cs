using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cartoraServer.Migrations
{
    public partial class mdcolum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImgeUrll_Products_Productid",
                table: "ImgeUrll");

            migrationBuilder.AlterColumn<int>(
                name: "Productid",
                table: "ImgeUrll",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ImgeUrll_Products_Productid",
                table: "ImgeUrll",
                column: "Productid",
                principalTable: "Products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImgeUrll_Products_Productid",
                table: "ImgeUrll");

            migrationBuilder.AlterColumn<int>(
                name: "Productid",
                table: "ImgeUrll",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ImgeUrll_Products_Productid",
                table: "ImgeUrll",
                column: "Productid",
                principalTable: "Products",
                principalColumn: "id");
        }
    }
}
