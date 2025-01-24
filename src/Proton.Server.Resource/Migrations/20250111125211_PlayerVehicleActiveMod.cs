using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proton.Server.Resource.Migrations
{
    /// <inheritdoc />
    public partial class PlayerVehicleActiveMod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerVehicleActiveMods",
                columns: table => new
                {
                    PlayerVehicleModId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerVehicleActiveMods", x => x.PlayerVehicleModId);
                    table.ForeignKey(
                        name: "FK_PlayerVehicleActiveMods_PlayerVehicleMods_PlayerVehicleModId",
                        column: x => x.PlayerVehicleModId,
                        principalTable: "PlayerVehicleMods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerVehicleActiveMods");
        }
    }
}
