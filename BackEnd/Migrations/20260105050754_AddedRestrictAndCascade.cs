using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddedRestrictAndCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentServiceItems_Items_ItemId",
                schema: "identity",
                table: "AppointmentServiceItems");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentServiceItems_Items_ItemId",
                schema: "identity",
                table: "AppointmentServiceItems",
                column: "ItemId",
                principalSchema: "identity",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentServiceItems_Items_ItemId",
                schema: "identity",
                table: "AppointmentServiceItems");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentServiceItems_Items_ItemId",
                schema: "identity",
                table: "AppointmentServiceItems",
                column: "ItemId",
                principalSchema: "identity",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
