using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class OriginalNameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentServiceItems_Appointments_AppointmentId",
                schema: "identity",
                table: "AppointmentServiceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentServiceItems_Items_ItemId",
                schema: "identity",
                table: "AppointmentServiceItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentServiceItems",
                schema: "identity",
                table: "AppointmentServiceItems");

            migrationBuilder.RenameTable(
                name: "AppointmentServiceItems",
                schema: "identity",
                newName: "AppointmentItems",
                newSchema: "identity");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentServiceItems_ItemId",
                schema: "identity",
                table: "AppointmentItems",
                newName: "IX_AppointmentItems_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentItems",
                schema: "identity",
                table: "AppointmentItems",
                columns: new[] { "AppointmentId", "ItemId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentItems_Appointments_AppointmentId",
                schema: "identity",
                table: "AppointmentItems",
                column: "AppointmentId",
                principalSchema: "identity",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentItems_Items_ItemId",
                schema: "identity",
                table: "AppointmentItems",
                column: "ItemId",
                principalSchema: "identity",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentItems_Appointments_AppointmentId",
                schema: "identity",
                table: "AppointmentItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentItems_Items_ItemId",
                schema: "identity",
                table: "AppointmentItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentItems",
                schema: "identity",
                table: "AppointmentItems");

            migrationBuilder.RenameTable(
                name: "AppointmentItems",
                schema: "identity",
                newName: "AppointmentServiceItems",
                newSchema: "identity");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentItems_ItemId",
                schema: "identity",
                table: "AppointmentServiceItems",
                newName: "IX_AppointmentServiceItems_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentServiceItems",
                schema: "identity",
                table: "AppointmentServiceItems",
                columns: new[] { "AppointmentId", "ItemId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentServiceItems_Appointments_AppointmentId",
                schema: "identity",
                table: "AppointmentServiceItems",
                column: "AppointmentId",
                principalSchema: "identity",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
