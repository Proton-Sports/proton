using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Proton.Server.Resource.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerVehicleWheelVariation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerVehicleWheelVariations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WheelVariationId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerVehicleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerVehicleWheelVariations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerVehicleWheelVariations_PlayerVehicles_PlayerVehicleId",
                        column: x => x.PlayerVehicleId,
                        principalTable: "PlayerVehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerVehicleWheelVariations_WheelVariations_WheelVariation~",
                        column: x => x.WheelVariationId,
                        principalTable: "WheelVariations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerVehicleActiveWheelVariations",
                columns: table => new
                {
                    PlayerVehicleWheelVariationId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerVehicleActiveWheelVariations", x => x.PlayerVehicleWheelVariationId);
                    table.ForeignKey(
                        name: "FK_PlayerVehicleActiveWheelVariations_PlayerVehicleWheelVariat~",
                        column: x => x.PlayerVehicleWheelVariationId,
                        principalTable: "PlayerVehicleWheelVariations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerVehicleWheelVariations_PlayerVehicleId_WheelVariation~",
                table: "PlayerVehicleWheelVariations",
                columns: new[] { "PlayerVehicleId", "WheelVariationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerVehicleWheelVariations_WheelVariationId",
                table: "PlayerVehicleWheelVariations",
                column: "WheelVariationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerVehicleActiveWheelVariations");

            migrationBuilder.DropTable(
                name: "PlayerVehicleWheelVariations");
        }
    }
}
