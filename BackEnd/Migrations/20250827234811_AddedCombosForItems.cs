using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddedCombosForItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComboItems",
                schema: "identity",
                columns: table => new
                {
                    ParentItemId = table.Column<long>(type: "bigint", nullable: false),
                    ChildItemId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboItems", x => new { x.ParentItemId, x.ChildItemId });
                    table.ForeignKey(
                        name: "FK_ComboItems_Items_ChildItemId",
                        column: x => x.ChildItemId,
                        principalSchema: "identity",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComboItems_Items_ParentItemId",
                        column: x => x.ParentItemId,
                        principalSchema: "identity",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComboItems_ChildItemId",
                schema: "identity",
                table: "ComboItems",
                column: "ChildItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComboItems",
                schema: "identity");
        }
    }
}
