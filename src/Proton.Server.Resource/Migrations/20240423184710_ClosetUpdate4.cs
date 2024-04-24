using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proton.Server.Resource.Migrations
{
    /// <inheritdoc />
    public partial class ClosetUpdate4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Closet_Cloth_ClothId",
                table: "Closet");

            migrationBuilder.DropForeignKey(
                name: "FK_Closet_Users_OwnerId",
                table: "Closet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cloth",
                table: "Cloth");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Closet",
                table: "Closet");

            migrationBuilder.RenameTable(
                name: "Cloth",
                newName: "Cloths");

            migrationBuilder.RenameTable(
                name: "Closet",
                newName: "Closets");

            migrationBuilder.RenameIndex(
                name: "IX_Closet_OwnerId",
                table: "Closets",
                newName: "IX_Closets_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Closet_ClothId",
                table: "Closets",
                newName: "IX_Closets_ClothId");

            migrationBuilder.AddColumn<bool>(
                name: "IsEquiped",
                table: "Closets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cloths",
                table: "Cloths",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Closets",
                table: "Closets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Closets_Cloths_ClothId",
                table: "Closets",
                column: "ClothId",
                principalTable: "Cloths",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Closets_Users_OwnerId",
                table: "Closets",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Closets_Cloths_ClothId",
                table: "Closets");

            migrationBuilder.DropForeignKey(
                name: "FK_Closets_Users_OwnerId",
                table: "Closets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cloths",
                table: "Cloths");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Closets",
                table: "Closets");

            migrationBuilder.DropColumn(
                name: "IsEquiped",
                table: "Closets");

            migrationBuilder.RenameTable(
                name: "Cloths",
                newName: "Cloth");

            migrationBuilder.RenameTable(
                name: "Closets",
                newName: "Closet");

            migrationBuilder.RenameIndex(
                name: "IX_Closets_OwnerId",
                table: "Closet",
                newName: "IX_Closet_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Closets_ClothId",
                table: "Closet",
                newName: "IX_Closet_ClothId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cloth",
                table: "Cloth",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Closet",
                table: "Closet",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Closet_Cloth_ClothId",
                table: "Closet",
                column: "ClothId",
                principalTable: "Cloth",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Closet_Users_OwnerId",
                table: "Closet",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
