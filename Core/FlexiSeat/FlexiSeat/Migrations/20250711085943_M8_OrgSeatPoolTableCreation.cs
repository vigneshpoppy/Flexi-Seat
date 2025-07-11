using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlexiSeat.Migrations
{
    /// <inheritdoc />
    public partial class M8_OrgSeatPoolTableCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrgSeatPools",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    ManagerADID = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    SeatsAllotted = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgSeatPools", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrgSeatPools_Users_ManagerADID",
                        column: x => x.ManagerADID,
                        principalTable: "Users",
                        principalColumn: "ADID");
                    table.ForeignKey(
                        name: "FK_OrgSeatPools_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrgSeatPools_ManagerADID",
                table: "OrgSeatPools",
                column: "ManagerADID");

            migrationBuilder.CreateIndex(
                name: "IX_OrgSeatPools_ZoneId",
                table: "OrgSeatPools",
                column: "ZoneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrgSeatPools");
        }
    }
}
