using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddedNullForCustomerIdForSpotPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpotPayments_Contacts_ContactId",
                schema: "identity",
                table: "SpotPayments");

            migrationBuilder.AlterColumn<long>(
                name: "ContactId",
                schema: "identity",
                table: "SpotPayments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_SpotPayments_Contacts_ContactId",
                schema: "identity",
                table: "SpotPayments",
                column: "ContactId",
                principalSchema: "identity",
                principalTable: "Contacts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpotPayments_Contacts_ContactId",
                schema: "identity",
                table: "SpotPayments");

            migrationBuilder.AlterColumn<long>(
                name: "ContactId",
                schema: "identity",
                table: "SpotPayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SpotPayments_Contacts_ContactId",
                schema: "identity",
                table: "SpotPayments",
                column: "ContactId",
                principalSchema: "identity",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
