using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cartoraServer.Migrations
{
    public partial class mdcolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MediaList",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaList",
                table: "Products");
        }
    }
}
