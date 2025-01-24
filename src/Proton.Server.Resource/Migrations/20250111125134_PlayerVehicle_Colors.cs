using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proton.Server.Resource.Migrations
{
    /// <inheritdoc />
    public partial class PlayerVehicle_Colors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "PrimaryColor_A",
                table: "PlayerVehicles",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "PrimaryColor_B",
                table: "PlayerVehicles",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "PrimaryColor_G",
                table: "PlayerVehicles",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "PrimaryColor_R",
                table: "PlayerVehicles",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "SecondaryColor_A",
                table: "PlayerVehicles",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "SecondaryColor_B",
                table: "PlayerVehicles",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "SecondaryColor_G",
                table: "PlayerVehicles",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "SecondaryColor_R",
                table: "PlayerVehicles",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryColor_A",
                table: "PlayerVehicles");

            migrationBuilder.DropColumn(
                name: "PrimaryColor_B",
                table: "PlayerVehicles");

            migrationBuilder.DropColumn(
                name: "PrimaryColor_G",
                table: "PlayerVehicles");

            migrationBuilder.DropColumn(
                name: "PrimaryColor_R",
                table: "PlayerVehicles");

            migrationBuilder.DropColumn(
                name: "SecondaryColor_A",
                table: "PlayerVehicles");

            migrationBuilder.DropColumn(
                name: "SecondaryColor_B",
                table: "PlayerVehicles");

            migrationBuilder.DropColumn(
                name: "SecondaryColor_G",
                table: "PlayerVehicles");

            migrationBuilder.DropColumn(
                name: "SecondaryColor_R",
                table: "PlayerVehicles");
        }
    }
}
