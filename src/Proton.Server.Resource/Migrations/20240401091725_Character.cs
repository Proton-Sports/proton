using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Proton.Server.Resource.Migrations
{
    /// <inheritdoc />
    public partial class Character : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UserId",
                table: "Characters",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Characters");
        }
    }
}
