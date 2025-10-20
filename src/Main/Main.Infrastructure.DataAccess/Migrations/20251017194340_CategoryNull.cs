using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Main.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CategoryNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Categories_CategoryId",
                table: "Inventories");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Inventories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId1",
                table: "Inventories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_CategoryId1",
                table: "Inventories",
                column: "CategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Categories_CategoryId",
                table: "Inventories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Categories_CategoryId1",
                table: "Inventories",
                column: "CategoryId1",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Categories_CategoryId",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Categories_CategoryId1",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_CategoryId1",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "Inventories");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Categories_CategoryId",
                table: "Inventories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
