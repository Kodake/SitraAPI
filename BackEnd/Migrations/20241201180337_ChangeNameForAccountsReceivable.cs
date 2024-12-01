using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameForAccountsReceivable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentReceipts_Receivables_AccountsReceivableId",
                table: "PaymentReceipts");

            migrationBuilder.RenameColumn(
                name: "AccountsReceivableId",
                table: "SaleInvoices",
                newName: "ReceivableId");

            migrationBuilder.RenameColumn(
                name: "AccountsReceivableId",
                table: "PaymentReceipts",
                newName: "ReceivableId");

            migrationBuilder.RenameColumn(
                name: "Account",
                table: "PaymentReceipts",
                newName: "TransactionType");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentReceipts_AccountsReceivableId",
                table: "PaymentReceipts",
                newName: "IX_PaymentReceipts_ReceivableId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentReceipts_Receivables_ReceivableId",
                table: "PaymentReceipts",
                column: "ReceivableId",
                principalTable: "Receivables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentReceipts_Receivables_ReceivableId",
                table: "PaymentReceipts");

            migrationBuilder.RenameColumn(
                name: "ReceivableId",
                table: "SaleInvoices",
                newName: "AccountsReceivableId");

            migrationBuilder.RenameColumn(
                name: "TransactionType",
                table: "PaymentReceipts",
                newName: "Account");

            migrationBuilder.RenameColumn(
                name: "ReceivableId",
                table: "PaymentReceipts",
                newName: "AccountsReceivableId");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentReceipts_ReceivableId",
                table: "PaymentReceipts",
                newName: "IX_PaymentReceipts_AccountsReceivableId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentReceipts_Receivables_AccountsReceivableId",
                table: "PaymentReceipts",
                column: "AccountsReceivableId",
                principalTable: "Receivables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
