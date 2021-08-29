using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace Solid_Price.Migrations {
    public partial class initial : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    VendorName = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    ContactName = table.Column<string>(type: "text", nullable: true),
                    ContactEmail = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Vendors", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StockItems",
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    MatType = table.Column<int>(type: "int", nullable: false),
                    ProfType = table.Column<int>(type: "int", nullable: false),
                    StockLength = table.Column<float>(type: "float", nullable: false),
                    InternalDescription = table.Column<string>(type: "text", nullable: true),
                    ExternalDescription = table.Column<string>(type: "text", nullable: true),
                    VendorID = table.Column<int>(type: "int", nullable: true),
                    CostPerFoot = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    VendorItemNumber = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_StockItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StockItems_Vendors_VendorID",
                        column: x => x.VendorID,
                        principalTable: "Vendors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CutItems",
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    StockItemID = table.Column<int>(type: "int", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Length = table.Column<float>(type: "float", nullable: false),
                    Angle1 = table.Column<float>(type: "float", nullable: false),
                    Angle2 = table.Column<float>(type: "float", nullable: false),
                    AngleDirection = table.Column<string>(type: "text", nullable: true),
                    AngleRotation = table.Column<string>(type: "text", nullable: true),
                    StickNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_CutItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CutItems_StockItems_StockItemID",
                        column: x => x.StockItemID,
                        principalTable: "StockItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    StockItemID = table.Column<int>(type: "int", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_OrderItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderItems_StockItems_StockItemID",
                        column: x => x.StockItemID,
                        principalTable: "StockItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CutItems_StockItemID",
                table: "CutItems",
                column: "StockItemID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_StockItemID",
                table: "OrderItems",
                column: "StockItemID");

            migrationBuilder.CreateIndex(
                name: "IX_StockItems_VendorID",
                table: "StockItems",
                column: "VendorID");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "CutItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "StockItems");

            migrationBuilder.DropTable(
                name: "Vendors");
        }
    }
}
