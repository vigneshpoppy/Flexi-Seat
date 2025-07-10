using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlexiSeat.Migrations
{
    /// <inheritdoc />
    public partial class M1_Reservations_TableCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserADID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReservedByADID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SeatID = table.Column<int>(type: "int", nullable: false),
                    ReservedDate = table.Column<DateTime>(type: "date", nullable: false),
                    CheckInTime = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CheckOutTime = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    InsertedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSmsSent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Reservations_Seats_SeatID",
                        column: x => x.SeatID,
                        principalTable: "Seats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_ReservedByADID",
                        column: x => x.ReservedByADID,
                        principalTable: "Users",
                        principalColumn: "ADID");
                    table.ForeignKey(
                        name: "FK_Reservations_Users_UserADID",
                        column: x => x.UserADID,
                        principalTable: "Users",
                        principalColumn: "ADID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReservedByADID",
                table: "Reservations",
                column: "ReservedByADID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_SeatID",
                table: "Reservations",
                column: "SeatID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UserADID",
                table: "Reservations",
                column: "UserADID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservations");
        }
    }
}
