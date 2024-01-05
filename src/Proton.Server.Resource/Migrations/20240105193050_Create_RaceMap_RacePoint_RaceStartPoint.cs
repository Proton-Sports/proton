using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Proton.Server.Resource.Migrations
{
    /// <inheritdoc />
    public partial class Create_RaceMap_RacePoint_RaceStartPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RaceMap",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceMap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RacePoint",
                columns: table => new
                {
                    MapId = table.Column<long>(type: "bigint", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Radius = table.Column<float>(type: "real", nullable: false),
                    Position_X = table.Column<float>(type: "real", nullable: false),
                    Position_Y = table.Column<float>(type: "real", nullable: false),
                    Position_Z = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RacePoint", x => new { x.MapId, x.Index });
                    table.ForeignKey(
                        name: "FK_RacePoint_RaceMap_MapId",
                        column: x => x.MapId,
                        principalTable: "RaceMap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaceStartPoint",
                columns: table => new
                {
                    MapId = table.Column<long>(type: "bigint", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Position_X = table.Column<float>(type: "real", nullable: false),
                    Position_Y = table.Column<float>(type: "real", nullable: false),
                    Position_Z = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceStartPoint", x => new { x.MapId, x.Index });
                    table.ForeignKey(
                        name: "FK_RaceStartPoint_RaceMap_MapId",
                        column: x => x.MapId,
                        principalTable: "RaceMap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RacePoint");

            migrationBuilder.DropTable(
                name: "RaceStartPoint");

            migrationBuilder.DropTable(
                name: "RaceMap");
        }
    }
}
