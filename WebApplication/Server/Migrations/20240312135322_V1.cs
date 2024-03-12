using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MENU_ITEM",
                columns: table => new
                {
                    MenuItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MENU_ITEM", x => x.MenuItemID);
                });

            migrationBuilder.CreateTable(
                name: "TABLE",
                columns: table => new
                {
                    TableID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Seats = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TABLE", x => x.TableID);
                });

            migrationBuilder.CreateTable(
                name: "WAITER",
                columns: table => new
                {
                    WaiterID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WAITER", x => x.WaiterID);
                });

            migrationBuilder.CreateTable(
                name: "GUEST",
                columns: table => new
                {
                    GuestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Money = table.Column<int>(type: "int", nullable: false),
                    HasAllergies = table.Column<bool>(type: "bit", nullable: false),
                    HasDiscount = table.Column<bool>(type: "bit", nullable: false),
                    TableID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GUEST", x => x.GuestID);
                    table.ForeignKey(
                        name: "FK_GUEST_TABLE_TableID",
                        column: x => x.TableID,
                        principalTable: "TABLE",
                        principalColumn: "TableID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ORDER",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuestID = table.Column<int>(type: "int", nullable: false),
                    WaiterID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDER", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_ORDER_GUEST_GuestID",
                        column: x => x.GuestID,
                        principalTable: "GUEST",
                        principalColumn: "GuestID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ORDER_WAITER_WaiterID",
                        column: x => x.WaiterID,
                        principalTable: "WAITER",
                        principalColumn: "WaiterID");
                });

            migrationBuilder.CreateTable(
                name: "ORDER_DETAILS",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    MenuItemID = table.Column<int>(type: "int", nullable: false),
                    OrderDetailID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDER_DETAILS", x => new { x.OrderID, x.MenuItemID });
                    table.ForeignKey(
                        name: "FK_ORDER_DETAILS_MENU_ITEM_MenuItemID",
                        column: x => x.MenuItemID,
                        principalTable: "MENU_ITEM",
                        principalColumn: "MenuItemID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ORDER_DETAILS_ORDER_OrderID",
                        column: x => x.OrderID,
                        principalTable: "ORDER",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GUEST_TableID",
                table: "GUEST",
                column: "TableID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_GuestID",
                table: "ORDER",
                column: "GuestID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_WaiterID",
                table: "ORDER",
                column: "WaiterID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_DETAILS_MenuItemID",
                table: "ORDER_DETAILS",
                column: "MenuItemID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ORDER_DETAILS");

            migrationBuilder.DropTable(
                name: "MENU_ITEM");

            migrationBuilder.DropTable(
                name: "ORDER");

            migrationBuilder.DropTable(
                name: "GUEST");

            migrationBuilder.DropTable(
                name: "WAITER");

            migrationBuilder.DropTable(
                name: "TABLE");
        }
    }
}
