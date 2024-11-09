using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBuyerIdForCustomerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleInvoices_Contacts_BuyerId",
                table: "SaleInvoices");

            migrationBuilder.RenameColumn(
                name: "BuyerId",
                table: "SaleInvoices",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_SaleInvoices_BuyerId",
                table: "SaleInvoices",
                newName: "IX_SaleInvoices_CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleInvoices_Contacts_CustomerId",
                table: "SaleInvoices",
                column: "CustomerId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleInvoices_Contacts_CustomerId",
                table: "SaleInvoices");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "SaleInvoices",
                newName: "BuyerId");

            migrationBuilder.RenameIndex(
                name: "IX_SaleInvoices_CustomerId",
                table: "SaleInvoices",
                newName: "IX_SaleInvoices_BuyerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleInvoices_Contacts_BuyerId",
                table: "SaleInvoices",
                column: "BuyerId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
