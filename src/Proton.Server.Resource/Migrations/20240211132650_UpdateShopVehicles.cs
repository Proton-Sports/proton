using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Proton.Server.Resource.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShopVehicles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RacePoint_RaceMaps_MapId",
                table: "RacePoint");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceStartPoint_RaceMaps_MapId",
                table: "RaceStartPoint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RaceStartPoint",
                table: "RaceStartPoint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RacePoint",
                table: "RacePoint");

            migrationBuilder.RenameTable(
                name: "RaceStartPoint",
                newName: "RaceStartPoints");

            migrationBuilder.RenameTable(
                name: "RacePoint",
                newName: "RacePoints");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RaceStartPoints",
                table: "RaceStartPoints",
                columns: new[] { "MapId", "Index" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RacePoints",
                table: "RacePoints",
                columns: new[] { "MapId", "Index" });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    AltVHash = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    VehicleGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    ColorDisplayname = table.Column<string>(type: "text", nullable: true),
                    AltVColor = table.Column<string>(type: "text", nullable: true),
                    PurchasedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_UserId",
                table: "Vehicles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RacePoints_RaceMaps_MapId",
                table: "RacePoints",
                column: "MapId",
                principalTable: "RaceMaps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RaceStartPoints_RaceMaps_MapId",
                table: "RaceStartPoints",
                column: "MapId",
                principalTable: "RaceMaps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RacePoints_RaceMaps_MapId",
                table: "RacePoints");

            migrationBuilder.DropForeignKey(
                name: "FK_RaceStartPoints_RaceMaps_MapId",
                table: "RaceStartPoints");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RaceStartPoints",
                table: "RaceStartPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RacePoints",
                table: "RacePoints");

            migrationBuilder.RenameTable(
                name: "RaceStartPoints",
                newName: "RaceStartPoint");

            migrationBuilder.RenameTable(
                name: "RacePoints",
                newName: "RacePoint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RaceStartPoint",
                table: "RaceStartPoint",
                columns: new[] { "MapId", "Index" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RacePoint",
                table: "RacePoint",
                columns: new[] { "MapId", "Index" });

            migrationBuilder.AddForeignKey(
                name: "FK_RacePoint_RaceMaps_MapId",
                table: "RacePoint",
                column: "MapId",
                principalTable: "RaceMaps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RaceStartPoint_RaceMaps_MapId",
                table: "RaceStartPoint",
                column: "MapId",
                principalTable: "RaceMaps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
