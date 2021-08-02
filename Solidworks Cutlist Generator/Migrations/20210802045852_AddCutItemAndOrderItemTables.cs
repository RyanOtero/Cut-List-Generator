using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace Solidworks_Cutlist_Generator.Migrations
{
    public partial class AddCutItemAndOrderItemTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockItems_Vendors_SpeakerId",
                table: "StockItems");

            migrationBuilder.DropIndex(
                name: "IX_StockItems_SpeakerId",
                table: "StockItems");

            migrationBuilder.DropColumn(
                name: "SpeakerId",
                table: "StockItems");

            migrationBuilder.CreateTable(
                name: "CutItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    StockItemID = table.Column<int>(type: "int", nullable: true),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Length = table.Column<float>(type: "float", nullable: false),
                    Angle1 = table.Column<float>(type: "float", nullable: false),
                    Angle2 = table.Column<float>(type: "float", nullable: false),
                    AngleDirection = table.Column<string>(type: "text", nullable: true),
                    AngleRotation = table.Column<string>(type: "text", nullable: true),
                    StickNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CutItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CutItems_StockItems_StockItemID",
                        column: x => x.StockItemID,
                        principalTable: "StockItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    StockItemID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderItems_StockItems_StockItemID",
                        column: x => x.StockItemID,
                        principalTable: "StockItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CutItems_StockItemID",
                table: "CutItems",
                column: "StockItemID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_StockItemID",
                table: "OrderItems",
                column: "StockItemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CutItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.AddColumn<int>(
                name: "SpeakerId",
                table: "StockItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockItems_SpeakerId",
                table: "StockItems",
                column: "SpeakerId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockItems_Vendors_SpeakerId",
                table: "StockItems",
                column: "SpeakerId",
                principalTable: "Vendors",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
