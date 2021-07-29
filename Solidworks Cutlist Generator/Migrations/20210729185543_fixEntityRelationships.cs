using Microsoft.EntityFrameworkCore.Migrations;

namespace Solidworks_Cutlist_Generator.Migrations
{
    public partial class fixEntityRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
