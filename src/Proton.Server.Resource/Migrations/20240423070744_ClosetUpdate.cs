using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Proton.Server.Resource.Migrations
{
    /// <inheritdoc />
    public partial class ClosetUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cloth",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsDlc = table.Column<bool>(type: "boolean", nullable: false),
                    IsProp = table.Column<bool>(type: "boolean", nullable: false),
                    Component = table.Column<int>(type: "integer", nullable: false),
                    Drawable = table.Column<int>(type: "integer", nullable: false),
                    Texture = table.Column<int>(type: "integer", nullable: false),
                    Palette = table.Column<int>(type: "integer", nullable: false),
                    DlcName = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    CurrentClothType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cloth", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Closet",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    ClothId = table.Column<long>(type: "bigint", nullable: false),
                    PurchaseTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Closet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Closet_Cloth_ClothId",
                        column: x => x.ClothId,
                        principalTable: "Cloth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Closet_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Closet_ClothId",
                table: "Closet",
                column: "ClothId");

            migrationBuilder.CreateIndex(
                name: "IX_Closet_OwnerId",
                table: "Closet",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Closet");

            migrationBuilder.DropTable(
                name: "Cloth");
        }
    }
}
