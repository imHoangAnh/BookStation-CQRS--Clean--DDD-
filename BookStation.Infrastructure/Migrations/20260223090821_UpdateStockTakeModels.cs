using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStockTakeModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VariantId",
                table: "OrderItems",
                newName: "BookVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_VariantId",
                table: "OrderItems",
                newName: "IX_OrderItems_BookVariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_BookVariants_BookVariantId",
                table: "OrderItems",
                column: "BookVariantId",
                principalTable: "BookVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_BookVariants_BookVariantId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "BookVariantId",
                table: "OrderItems",
                newName: "VariantId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_BookVariantId",
                table: "OrderItems",
                newName: "IX_OrderItems_VariantId");
        }
    }
}
