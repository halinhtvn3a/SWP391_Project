using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplication2.Data.Migrations
{
	/// <inheritdoc />
	public partial class InitialCreate : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "AspNetRoles",
				columns: table => new
				{
					Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetRoles", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUsers",
				columns: table => new
				{
					Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
					UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
					EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
					PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
					SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
					TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
					LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
					LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
					AccessFailedCount = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUsers", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Branches",
				columns: table => new
				{
					BranchId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
					Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
					Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
					Picture = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
					OpenTime = table.Column<TimeSpan>(type: "time", nullable: false),
					CloseTime = table.Column<TimeSpan>(type: "time", nullable: false),
					OpenDay = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
					Status = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Branches", x => x.BranchId);
				});

			migrationBuilder.CreateTable(
				name: "AspNetRoleClaims",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserClaims",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
					table.ForeignKey(
						name: "FK_AspNetUserClaims_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserLogins",
				columns: table => new
				{
					LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
					ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
					ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
					table.ForeignKey(
						name: "FK_AspNetUserLogins_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserRoles",
				columns: table => new
				{
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
						column: x => x.RoleId,
						principalTable: "AspNetRoles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_AspNetUserRoles_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AspNetUserTokens",
				columns: table => new
				{
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
					LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
					Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
					table.ForeignKey(
						name: "FK_AspNetUserTokens_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "UserDetails",
				columns: table => new
				{
					UserDetailsId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
					Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
					FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
					Status = table.Column<bool>(type: "bit", nullable: false),
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_UserDetails", x => x.UserDetailsId);
					table.ForeignKey(
						name: "FK_UserDetails_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id");
				});

			migrationBuilder.CreateTable(
				name: "Courts",
				columns: table => new
				{
					CourtId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
					BranchId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
					CourtName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
					Status = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Courts", x => x.CourtId);
					table.ForeignKey(
						name: "FK_Courts_Branches_BranchId",
						column: x => x.BranchId,
						principalTable: "Branches",
						principalColumn: "BranchId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Reviews",
				columns: table => new
				{
					ReviewId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
					ReviewText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
					ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
					Rating = table.Column<int>(type: "int", nullable: true),
					Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
					CourtId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Reviews", x => x.ReviewId);
					table.ForeignKey(
						name: "FK_Reviews_AspNetUsers_Id",
						column: x => x.Id,
						principalTable: "AspNetUsers",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_Reviews_Courts_CourtId",
						column: x => x.CourtId,
						principalTable: "Courts",
						principalColumn: "CourtId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "TimeSlots",
				columns: table => new
				{
					SlotId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
					CourtId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
					SlotDate = table.Column<DateTime>(type: "datetime2", nullable: false),
					SlotStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
					SlotEndTime = table.Column<TimeSpan>(type: "time", nullable: false),
					IsAvailable = table.Column<bool>(type: "bit", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_TimeSlots", x => x.SlotId);
					table.ForeignKey(
						name: "FK_TimeSlots_Courts_CourtId",
						column: x => x.CourtId,
						principalTable: "Courts",
						principalColumn: "CourtId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Bookings",
				columns: table => new
				{
					BookingId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
					Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
					SlotId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
					BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
					Check = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Bookings", x => x.BookingId);
					table.ForeignKey(
						name: "FK_Bookings_AspNetUsers_Id",
						column: x => x.Id,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Bookings_TimeSlots_SlotId",
						column: x => x.SlotId,
						principalTable: "TimeSlots",
						principalColumn: "SlotId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Payments",
				columns: table => new
				{
					PaymentId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
					BookingId = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
					PaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
					PaymentMessage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
					PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
					PaymentSignature = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Payments", x => x.PaymentId);
					table.ForeignKey(
						name: "FK_Payments_Bookings_BookingId",
						column: x => x.BookingId,
						principalTable: "Bookings",
						principalColumn: "BookingId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.InsertData(
				table: "AspNetRoles",
				columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
				values: new object[,]
				{
					{ "R001", "128d8841-3ff9-497b-9b2d-3b05a9e0d369", "Admin", "ADMIN" },
					{ "R002", "01d1bd17-2d67-423e-8d2d-9affdd6adfa7", "Staff", "STAFF" },
					{ "R003", "c9bd0c01-1b03-4f23-9db7-afcf79705200", "Customer", "CUSTOMER" }
				});

			migrationBuilder.CreateIndex(
				name: "IX_AspNetRoleClaims_RoleId",
				table: "AspNetRoleClaims",
				column: "RoleId");

			migrationBuilder.CreateIndex(
				name: "RoleNameIndex",
				table: "AspNetRoles",
				column: "NormalizedName",
				unique: true,
				filter: "[NormalizedName] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserClaims_UserId",
				table: "AspNetUserClaims",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserLogins_UserId",
				table: "AspNetUserLogins",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUserRoles_RoleId",
				table: "AspNetUserRoles",
				column: "RoleId");

			migrationBuilder.CreateIndex(
				name: "EmailIndex",
				table: "AspNetUsers",
				column: "NormalizedEmail");

			migrationBuilder.CreateIndex(
				name: "UserNameIndex",
				table: "AspNetUsers",
				column: "NormalizedUserName",
				unique: true,
				filter: "[NormalizedUserName] IS NOT NULL");

			migrationBuilder.CreateIndex(
				name: "IX_Bookings_Id",
				table: "Bookings",
				column: "Id");

			migrationBuilder.CreateIndex(
				name: "IX_Bookings_SlotId",
				table: "Bookings",
				column: "SlotId");

			migrationBuilder.CreateIndex(
				name: "IX_Courts_BranchId",
				table: "Courts",
				column: "BranchId");

			migrationBuilder.CreateIndex(
				name: "IX_Payments_BookingId",
				table: "Payments",
				column: "BookingId");

			migrationBuilder.CreateIndex(
				name: "IX_Reviews_CourtId",
				table: "Reviews",
				column: "CourtId");

			migrationBuilder.CreateIndex(
				name: "IX_Reviews_Id",
				table: "Reviews",
				column: "Id");

			migrationBuilder.CreateIndex(
				name: "IX_TimeSlots_CourtId",
				table: "TimeSlots",
				column: "CourtId");

			migrationBuilder.CreateIndex(
				name: "IX_UserDetails_UserId",
				table: "UserDetails",
				column: "UserId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "AspNetRoleClaims");

			migrationBuilder.DropTable(
				name: "AspNetUserClaims");

			migrationBuilder.DropTable(
				name: "AspNetUserLogins");

			migrationBuilder.DropTable(
				name: "AspNetUserRoles");

			migrationBuilder.DropTable(
				name: "AspNetUserTokens");

			migrationBuilder.DropTable(
				name: "Payments");

			migrationBuilder.DropTable(
				name: "Reviews");

			migrationBuilder.DropTable(
				name: "UserDetails");

			migrationBuilder.DropTable(
				name: "AspNetRoles");

			migrationBuilder.DropTable(
				name: "Bookings");

			migrationBuilder.DropTable(
				name: "AspNetUsers");

			migrationBuilder.DropTable(
				name: "TimeSlots");

			migrationBuilder.DropTable(
				name: "Courts");

			migrationBuilder.DropTable(
				name: "Branches");
		}
	}
}
