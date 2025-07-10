using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlexiSeat.Migrations
{
    /// <inheritdoc />
    public partial class M1_Users_TableCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ADID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BadgeID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    LeadADID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ManagerADID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ADID);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Users_LeadADID",
                        column: x => x.LeadADID,
                        principalTable: "Users",
                        principalColumn: "ADID");
                    table.ForeignKey(
                        name: "FK_Users_Users_ManagerADID",
                        column: x => x.ManagerADID,
                        principalTable: "Users",
                        principalColumn: "ADID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_LeadADID",
                table: "Users",
                column: "LeadADID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ManagerADID",
                table: "Users",
                column: "ManagerADID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
