using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Proton.Server.Resource.Migrations
{
    /// <inheritdoc />
    public partial class race_restoration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRaceRestorations",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Lap = table.Column<int>(type: "integer", nullable: false),
                    AccumulatedDistance = table.Column<float>(type: "real", nullable: false),
                    PartialDistance = table.Column<float>(type: "real", nullable: false),
                    NextRacePointIndex = table.Column<int>(type: "integer", nullable: true),
                    FinishTime = table.Column<long>(type: "bigint", nullable: false),
                    X = table.Column<float>(type: "real", nullable: false),
                    Y = table.Column<float>(type: "real", nullable: false),
                    Z = table.Column<float>(type: "real", nullable: false),
                    Roll = table.Column<float>(type: "real", nullable: false),
                    Pitch = table.Column<float>(type: "real", nullable: false),
                    Yaw = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRaceRestorations", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserRaceRestorations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRacePointRestoration",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Lap = table.Column<int>(type: "integer", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRacePointRestoration", x => new { x.UserId, x.Index });
                    table.ForeignKey(
                        name: "FK_UserRacePointRestoration_UserRaceRestorations_UserId",
                        column: x => x.UserId,
                        principalTable: "UserRaceRestorations",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRacePointRestoration");

            migrationBuilder.DropTable(
                name: "UserRaceRestorations");
        }
    }
}
