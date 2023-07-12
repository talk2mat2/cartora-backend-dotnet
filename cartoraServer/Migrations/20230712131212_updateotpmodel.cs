using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cartoraServer.Migrations
{
    public partial class updateotpmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OtpModel");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "OtpModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "OtpModel");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "OtpModel",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
