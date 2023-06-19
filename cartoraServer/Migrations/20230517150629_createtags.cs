using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cartoraServer.Migrations
{
    public partial class createtags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Vehicles = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Properties = table.Column<bool>(type: "bit", nullable: false),
                    Electronics = table.Column<bool>(type: "bit", nullable: false),
                    Furnitures = table.Column<bool>(type: "bit", nullable: false),
                    Health = table.Column<bool>(type: "bit", nullable: false),
                    Fashion = table.Column<bool>(type: "bit", nullable: false),
                    Sport = table.Column<bool>(type: "bit", nullable: false),
                    Services = table.Column<bool>(type: "bit", nullable: false),
                    Jobs = table.Column<bool>(type: "bit", nullable: false),
                    Babies = table.Column<bool>(type: "bit", nullable: false),
                    Agric = table.Column<bool>(type: "bit", nullable: false),
                    Repairs = table.Column<bool>(type: "bit", nullable: false),
                    Equipments = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.id);
                    table.ForeignKey(
                        name: "FK_Tags_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UserId",
                table: "Tags",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
