using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class fixReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemFieldValues_InventoryFields_InventoryFieldId",
                table: "ItemFieldValues");

            migrationBuilder.AlterColumn<string>(
                name: "CustomId",
                table: "Items",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Items_InventoryId_CustomId",
                table: "Items",
                columns: new[] { "InventoryId", "CustomId" },
                unique: true,
                filter: "[CustomId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemFieldValues_InventoryFields_InventoryFieldId",
                table: "ItemFieldValues",
                column: "InventoryFieldId",
                principalTable: "InventoryFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemFieldValues_InventoryFields_InventoryFieldId",
                table: "ItemFieldValues");

            migrationBuilder.DropIndex(
                name: "IX_Items_InventoryId_CustomId",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "CustomId",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemFieldValues_InventoryFields_InventoryFieldId",
                table: "ItemFieldValues",
                column: "InventoryFieldId",
                principalTable: "InventoryFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
