using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proton.Server.Resource.Migrations
{
    /// <inheritdoc />
    public partial class ModifyBanRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "BanRecords",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BanRecords",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "BanRecords");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BanRecords",
                newName: "Value");
        }
    }
}
