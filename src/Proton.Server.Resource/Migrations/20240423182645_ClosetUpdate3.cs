using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proton.Server.Resource.Migrations
{
    /// <inheritdoc />
    public partial class ClosetUpdate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentClothType",
                table: "Cloth");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Cloth",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Cloth",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Cloth");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Cloth");

            migrationBuilder.AddColumn<int>(
                name: "CurrentClothType",
                table: "Cloth",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
