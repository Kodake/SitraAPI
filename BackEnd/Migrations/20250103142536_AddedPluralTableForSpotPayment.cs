using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddedPluralTableForSpotPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpotPayment_Contacts_ContactId",
                table: "SpotPayment");

            migrationBuilder.DropForeignKey(
                name: "FK_SpotPayment_SaleInvoices_SaleInvoiceId",
                table: "SpotPayment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpotPayment",
                table: "SpotPayment");

            migrationBuilder.RenameTable(
                name: "SpotPayment",
                newName: "SpotPayments");

            migrationBuilder.RenameIndex(
                name: "IX_SpotPayment_SaleInvoiceId",
                table: "SpotPayments",
                newName: "IX_SpotPayments_SaleInvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_SpotPayment_ContactId",
                table: "SpotPayments",
                newName: "IX_SpotPayments_ContactId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpotPayments",
                table: "SpotPayments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SpotPayments_Contacts_ContactId",
                table: "SpotPayments",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SpotPayments_SaleInvoices_SaleInvoiceId",
                table: "SpotPayments",
                column: "SaleInvoiceId",
                principalTable: "SaleInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpotPayments_Contacts_ContactId",
                table: "SpotPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_SpotPayments_SaleInvoices_SaleInvoiceId",
                table: "SpotPayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpotPayments",
                table: "SpotPayments");

            migrationBuilder.RenameTable(
                name: "SpotPayments",
                newName: "SpotPayment");

            migrationBuilder.RenameIndex(
                name: "IX_SpotPayments_SaleInvoiceId",
                table: "SpotPayment",
                newName: "IX_SpotPayment_SaleInvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_SpotPayments_ContactId",
                table: "SpotPayment",
                newName: "IX_SpotPayment_ContactId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpotPayment",
                table: "SpotPayment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SpotPayment_Contacts_ContactId",
                table: "SpotPayment",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SpotPayment_SaleInvoices_SaleInvoiceId",
                table: "SpotPayment",
                column: "SaleInvoiceId",
                principalTable: "SaleInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
