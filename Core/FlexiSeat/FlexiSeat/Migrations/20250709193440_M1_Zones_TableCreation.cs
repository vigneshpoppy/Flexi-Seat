using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlexiSeat.Migrations
{
    /// <inheritdoc />
    public partial class M1_Zones_TableCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LocationName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ManagerADID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Zones_Users_ManagerADID",
                        column: x => x.ManagerADID,
                        principalTable: "Users",
                        principalColumn: "ADID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Zones_ManagerADID",
                table: "Zones",
                column: "ManagerADID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Zones");
        }
    }
}
