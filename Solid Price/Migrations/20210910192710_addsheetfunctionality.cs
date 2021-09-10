using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace SolidPrice.Migrations
{
    public partial class addsheetfunctionality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SheetStockItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    MatType = table.Column<int>(type: "int", nullable: false),
                    StockLengthInInches = table.Column<float>(type: "float", nullable: false),
                    StockWidthInInches = table.Column<float>(type: "float", nullable: false),
                    Thickness = table.Column<float>(type: "float", nullable: false),
                    Finish = table.Column<string>(type: "text", nullable: true),
                    InternalDescription = table.Column<string>(type: "text", nullable: true),
                    ExternalDescription = table.Column<string>(type: "text", nullable: true),
                    VendorID = table.Column<int>(type: "int", nullable: true),
                    CostPerSqFoot = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    VendorItemNumber = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheetStockItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SheetStockItems_Vendors_VendorID",
                        column: x => x.VendorID,
                        principalTable: "Vendors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SheetCutItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SheetStockItemID = table.Column<int>(type: "int", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Length = table.Column<float>(type: "float", nullable: false),
                    Width = table.Column<float>(type: "float", nullable: false),
                    SheetNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheetCutItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SheetCutItems_SheetStockItems_SheetStockItemID",
                        column: x => x.SheetStockItemID,
                        principalTable: "SheetStockItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SheetOrderItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    SheetStockItemID = table.Column<int>(type: "int", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheetOrderItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SheetOrderItems_SheetStockItems_SheetStockItemID",
                        column: x => x.SheetStockItemID,
                        principalTable: "SheetStockItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SheetCutItems_SheetStockItemID",
                table: "SheetCutItems",
                column: "SheetStockItemID");

            migrationBuilder.CreateIndex(
                name: "IX_SheetOrderItems_SheetStockItemID",
                table: "SheetOrderItems",
                column: "SheetStockItemID");

            migrationBuilder.CreateIndex(
                name: "IX_SheetStockItems_VendorID",
                table: "SheetStockItems",
                column: "VendorID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SheetCutItems");

            migrationBuilder.DropTable(
                name: "SheetOrderItems");

            migrationBuilder.DropTable(
                name: "SheetStockItems");
        }
    }
}
