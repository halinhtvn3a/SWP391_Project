using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourtManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Court",
                columns: table => new
                {
                    CourtId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    Location = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Picture = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Opentime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Closetime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Openday = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Court__C3A67C9A3210AC12", x => x.CourtId);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__8AFACE1A54A3586A", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    Username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    RoleId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true),
                    Balance = table.Column<decimal>(type: "money", nullable: true),
                    Qrcode = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__1788CC4C21571AF2", x => x.UserId);
                    table.ForeignKey(
                        name: "FK__User__RoleId__37A5467C",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId");
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    BookingId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    UserId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true),
                    Bookingdate = table.Column<DateOnly>(type: "date", nullable: false),
                    Totalprice = table.Column<int>(type: "int", nullable: false),
                    Check = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Booking__73951AED157D3D7C", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK__Booking__UserId__31EC6D26",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserDetail",
                columns: table => new
                {
                    UserDetailId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    Email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Fullname = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    UserId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserDeta__564F56B24AA06B56", x => x.UserDetailId);
                    table.ForeignKey(
                        name: "FK__UserDetai__UserI__38996AB5",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "CourtSlot",
                columns: table => new
                {
                    CourtslotId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    CourtId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Starttime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Endtime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Price = table.Column<decimal>(type: "money", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    BookingId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CourtSlo__C632D55B123E7212", x => x.CourtslotId);
                    table.ForeignKey(
                        name: "FK__CourtSlot__Booki__35BCFE0A",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__CourtSlot__Court__34C8D9D1",
                        column: x => x.CourtId,
                        principalTable: "Court",
                        principalColumn: "CourtId");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    Method = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    BookingId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true),
                    Paytime = table.Column<DateTime>(type: "datetime", nullable: false),
                    ExternalVnPayTransactionCode = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__9B556A384382C024", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK__Payment__Booking__36B12243",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "BookingId");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    ReplyText = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    DateReplied = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true),
                    CourtSlotId = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Comments__C3B4DFCA40228F9D", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK__Comments__CourtS__33D4B598",
                        column: x => x.CourtSlotId,
                        principalTable: "CourtSlot",
                        principalColumn: "CourtslotId");
                    table.ForeignKey(
                        name: "FK__Comments__UserId__32E0915F",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_UserId",
                table: "Booking",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CourtSlotId",
                table: "Comments",
                column: "CourtSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtSlot_BookingId",
                table: "CourtSlot",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_CourtSlot_CourtId",
                table: "CourtSlot",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_BookingId",
                table: "Payment",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDetail_UserId",
                table: "UserDetail",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "UserDetail");

            migrationBuilder.DropTable(
                name: "CourtSlot");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "Court");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
