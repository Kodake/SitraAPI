using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubAndModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ModelId",
                schema: "identity",
                table: "Items",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SubCategoryId",
                schema: "identity",
                table: "Items",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_ModelId",
                schema: "identity",
                table: "Items",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_SubCategoryId",
                schema: "identity",
                table: "Items",
                column: "SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Models_ModelId",
                schema: "identity",
                table: "Items",
                column: "ModelId",
                principalSchema: "identity",
                principalTable: "Models",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_SubCategories_SubCategoryId",
                schema: "identity",
                table: "Items",
                column: "SubCategoryId",
                principalSchema: "identity",
                principalTable: "SubCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Models_ModelId",
                schema: "identity",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_SubCategories_SubCategoryId",
                schema: "identity",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_ModelId",
                schema: "identity",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_SubCategoryId",
                schema: "identity",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ModelId",
                schema: "identity",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                schema: "identity",
                table: "Items");
        }
    }
}
