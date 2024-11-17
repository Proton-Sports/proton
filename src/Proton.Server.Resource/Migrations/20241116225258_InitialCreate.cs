using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Proton.Server.Resource.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cloths",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsDlc = table.Column<bool>(type: "boolean", nullable: false),
                    IsProp = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Component = table.Column<int>(type: "integer", nullable: false),
                    Drawable = table.Column<int>(type: "integer", nullable: false),
                    Texture = table.Column<int>(type: "integer", nullable: false),
                    Palette = table.Column<int>(type: "integer", nullable: false),
                    DlcName = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cloths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RaceMaps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IplName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceMaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    DiscordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Money = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    AltVHash = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RacePoints",
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
                    table.PrimaryKey("PK_RacePoints", x => new { x.MapId, x.Index });
                    table.ForeignKey(
                        name: "FK_RacePoints_RaceMaps_MapId",
                        column: x => x.MapId,
                        principalTable: "RaceMaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaceStartPoints",
                columns: table => new
                {
                    MapId = table.Column<long>(type: "bigint", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Position_X = table.Column<float>(type: "real", nullable: false),
                    Position_Y = table.Column<float>(type: "real", nullable: false),
                    Position_Z = table.Column<float>(type: "real", nullable: false),
                    Rotation_X = table.Column<float>(type: "real", nullable: false),
                    Rotation_Y = table.Column<float>(type: "real", nullable: false),
                    Rotation_Z = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceStartPoints", x => new { x.MapId, x.Index });
                    table.ForeignKey(
                        name: "FK_RaceStartPoints_RaceMaps_MapId",
                        column: x => x.MapId,
                        principalTable: "RaceMaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CharacterGender = table.Column<int>(type: "integer", nullable: false),
                    FaceFather = table.Column<int>(type: "integer", nullable: false),
                    FaceMother = table.Column<int>(type: "integer", nullable: false),
                    SkinFather = table.Column<int>(type: "integer", nullable: false),
                    SkinMother = table.Column<int>(type: "integer", nullable: false),
                    SkinMix = table.Column<float>(type: "real", nullable: false),
                    FaceMix = table.Column<float>(type: "real", nullable: false),
                    EyeColor = table.Column<int>(type: "integer", nullable: false),
                    FaceFeatures = table.Column<string>(type: "text", nullable: false),
                    FaceOverlays = table.Column<string>(type: "text", nullable: false),
                    HairDrawable = table.Column<int>(type: "integer", nullable: false),
                    FirstHairColor = table.Column<int>(type: "integer", nullable: false),
                    SecondHairColor = table.Column<int>(type: "integer", nullable: false),
                    FacialHair = table.Column<int>(type: "integer", nullable: false),
                    FirstFacialHairColor = table.Column<int>(type: "integer", nullable: false),
                    SecondFacialHairColor = table.Column<int>(type: "integer", nullable: false),
                    FacialHairOpacity = table.Column<float>(type: "real", nullable: false),
                    Eyebrows = table.Column<int>(type: "integer", nullable: false),
                    EyebrowsColor = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Closets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    ClothId = table.Column<long>(type: "bigint", nullable: false),
                    IsEquiped = table.Column<bool>(type: "boolean", nullable: false),
                    PurchaseTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Closets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Closets_Cloths_ClothId",
                        column: x => x.ClothId,
                        principalTable: "Cloths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Closets_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TimestampLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimestampLogout = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Ipv4 = table.Column<string>(type: "text", nullable: false),
                    Ipv6 = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    Yaw = table.Column<float>(type: "real", nullable: false),
                    VehicleModel = table.Column<long>(type: "bigint", nullable: false)
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
                name: "Garages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    VehicleId = table.Column<long>(type: "bigint", nullable: false),
                    AltVColor = table.Column<int>(type: "integer", nullable: false),
                    PurchasedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Garages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Garages_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Garages_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRacePointRestoration",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UserId",
                table: "Characters",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Closets_ClothId",
                table: "Closets",
                column: "ClothId");

            migrationBuilder.CreateIndex(
                name: "IX_Closets_OwnerId",
                table: "Closets",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Garages_OwnerId",
                table: "Garages",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Garages_VehicleId",
                table: "Garages",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Closets");

            migrationBuilder.DropTable(
                name: "Garages");

            migrationBuilder.DropTable(
                name: "RacePoints");

            migrationBuilder.DropTable(
                name: "RaceStartPoints");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "UserRacePointRestoration");

            migrationBuilder.DropTable(
                name: "Cloths");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "RaceMaps");

            migrationBuilder.DropTable(
                name: "UserRaceRestorations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
