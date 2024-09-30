using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proton.Server.Resource.Migrations
{
    /// <inheritdoc />
    public partial class Add_RaceRestoration_VehicleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "VehicleModel",
                table: "UserRaceRestorations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleModel",
                table: "UserRaceRestorations");
        }
    }
}
