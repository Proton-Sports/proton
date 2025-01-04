using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proton.Server.Resource.Migrations
{
    /// <inheritdoc />
    public partial class AddModName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerVehicleMod_Mod_ModId",
                table: "PlayerVehicleMod");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerVehicleMod_PlayerVehicles_PlayerVehicleId",
                table: "PlayerVehicleMod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerVehicleMod",
                table: "PlayerVehicleMod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mod",
                table: "Mod");

            migrationBuilder.RenameTable(
                name: "PlayerVehicleMod",
                newName: "PlayerVehicleMods");

            migrationBuilder.RenameTable(
                name: "Mod",
                newName: "Mods");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerVehicleMod_PlayerVehicleId_ModId",
                table: "PlayerVehicleMods",
                newName: "IX_PlayerVehicleMods_PlayerVehicleId_ModId");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerVehicleMod_ModId",
                table: "PlayerVehicleMods",
                newName: "IX_PlayerVehicleMods_ModId");

            migrationBuilder.RenameIndex(
                name: "IX_Mod_Model",
                table: "Mods",
                newName: "IX_Mods_Model");

            migrationBuilder.RenameIndex(
                name: "IX_Mod_Category",
                table: "Mods",
                newName: "IX_Mods_Category");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Mods",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerVehicleMods",
                table: "PlayerVehicleMods",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mods",
                table: "Mods",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerVehicleMods_Mods_ModId",
                table: "PlayerVehicleMods",
                column: "ModId",
                principalTable: "Mods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerVehicleMods_PlayerVehicles_PlayerVehicleId",
                table: "PlayerVehicleMods",
                column: "PlayerVehicleId",
                principalTable: "PlayerVehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerVehicleMods_Mods_ModId",
                table: "PlayerVehicleMods");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerVehicleMods_PlayerVehicles_PlayerVehicleId",
                table: "PlayerVehicleMods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlayerVehicleMods",
                table: "PlayerVehicleMods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mods",
                table: "Mods");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Mods");

            migrationBuilder.RenameTable(
                name: "PlayerVehicleMods",
                newName: "PlayerVehicleMod");

            migrationBuilder.RenameTable(
                name: "Mods",
                newName: "Mod");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerVehicleMods_PlayerVehicleId_ModId",
                table: "PlayerVehicleMod",
                newName: "IX_PlayerVehicleMod_PlayerVehicleId_ModId");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerVehicleMods_ModId",
                table: "PlayerVehicleMod",
                newName: "IX_PlayerVehicleMod_ModId");

            migrationBuilder.RenameIndex(
                name: "IX_Mods_Model",
                table: "Mod",
                newName: "IX_Mod_Model");

            migrationBuilder.RenameIndex(
                name: "IX_Mods_Category",
                table: "Mod",
                newName: "IX_Mod_Category");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlayerVehicleMod",
                table: "PlayerVehicleMod",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mod",
                table: "Mod",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerVehicleMod_Mod_ModId",
                table: "PlayerVehicleMod",
                column: "ModId",
                principalTable: "Mod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerVehicleMod_PlayerVehicles_PlayerVehicleId",
                table: "PlayerVehicleMod",
                column: "PlayerVehicleId",
                principalTable: "PlayerVehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
