using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlexiSeat.Migrations
{
    /// <inheritdoc />
    public partial class M7_UpdatedZoneTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zones_Users_ManagerADID",
                table: "Zones");

            migrationBuilder.DropIndex(
                name: "IX_Zones_ManagerADID",
                table: "Zones");

            migrationBuilder.DropColumn(
                name: "ManagerADID",
                table: "Zones");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ManagerADID",
                table: "Zones",
                type: "nvarchar(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zones_ManagerADID",
                table: "Zones",
                column: "ManagerADID");

            migrationBuilder.AddForeignKey(
                name: "FK_Zones_Users_ManagerADID",
                table: "Zones",
                column: "ManagerADID",
                principalTable: "Users",
                principalColumn: "ADID");
        }
    }
}
