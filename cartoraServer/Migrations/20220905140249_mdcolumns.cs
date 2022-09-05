using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cartoraServer.Migrations
{
    public partial class mdcolumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaList",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "ImgeUrll",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Productid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImgeUrll", x => x.id);
                    table.ForeignKey(
                        name: "FK_ImgeUrll_Products_Productid",
                        column: x => x.Productid,
                        principalTable: "Products",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImgeUrll_Productid",
                table: "ImgeUrll",
                column: "Productid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImgeUrll");

            migrationBuilder.AddColumn<string>(
                name: "MediaList",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
