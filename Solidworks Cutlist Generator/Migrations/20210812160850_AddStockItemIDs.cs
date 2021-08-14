using Microsoft.EntityFrameworkCore.Migrations;

namespace Solidworks_Cutlist_Generator.Migrations
{
    public partial class AddStockItemIDs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CutItems_StockItems_StockItemID",
                table: "CutItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_StockItems_StockItemID",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "StockItemID",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StockItemID",
                table: "CutItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CutItems_StockItems_StockItemID",
                table: "CutItems",
                column: "StockItemID",
                principalTable: "StockItems",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_StockItems_StockItemID",
                table: "OrderItems",
                column: "StockItemID",
                principalTable: "StockItems",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CutItems_StockItems_StockItemID",
                table: "CutItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_StockItems_StockItemID",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "StockItemID",
                table: "OrderItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StockItemID",
                table: "CutItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CutItems_StockItems_StockItemID",
                table: "CutItems",
                column: "StockItemID",
                principalTable: "StockItems",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_StockItems_StockItemID",
                table: "OrderItems",
                column: "StockItemID",
                principalTable: "StockItems",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
