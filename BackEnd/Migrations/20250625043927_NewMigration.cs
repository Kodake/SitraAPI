using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payables_Contacts_ContactId",
                schema: "identity",
                table: "Payables");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_Contacts_ContactId",
                schema: "identity",
                table: "Quotations");

            migrationBuilder.DropForeignKey(
                name: "FK_Receivables_Contacts_ContactId",
                schema: "identity",
                table: "Receivables");

            migrationBuilder.DropForeignKey(
                name: "FK_SpotPayables_Contacts_ContactId",
                schema: "identity",
                table: "SpotPayables");

            migrationBuilder.DropIndex(
                name: "IX_SpotPayables_InvoiceId",
                schema: "identity",
                table: "SpotPayables");

            migrationBuilder.DropIndex(
                name: "IX_Receivables_InvoiceId",
                schema: "identity",
                table: "Receivables");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_InvoiceId",
                schema: "identity",
                table: "Quotations");

            migrationBuilder.AlterColumn<long>(
                name: "InvoiceId",
                schema: "identity",
                table: "SpotPayables",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ContactId",
                schema: "identity",
                table: "SpotPayables",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "InvoiceId",
                schema: "identity",
                table: "Receivables",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ContactId",
                schema: "identity",
                table: "Receivables",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "InvoiceId",
                schema: "identity",
                table: "Quotations",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ContactId",
                schema: "identity",
                table: "Quotations",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ContactId",
                schema: "identity",
                table: "Payables",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "CustomerId",
                schema: "identity",
                table: "Invoices",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_SpotPayables_InvoiceId",
                schema: "identity",
                table: "SpotPayables",
                column: "InvoiceId",
                unique: true,
                filter: "[InvoiceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Receivables_InvoiceId",
                schema: "identity",
                table: "Receivables",
                column: "InvoiceId",
                unique: true,
                filter: "[InvoiceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_InvoiceId",
                schema: "identity",
                table: "Quotations",
                column: "InvoiceId",
                unique: true,
                filter: "[InvoiceId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Payables_Contacts_ContactId",
                schema: "identity",
                table: "Payables",
                column: "ContactId",
                principalSchema: "identity",
                principalTable: "Contacts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_Contacts_ContactId",
                schema: "identity",
                table: "Quotations",
                column: "ContactId",
                principalSchema: "identity",
                principalTable: "Contacts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Receivables_Contacts_ContactId",
                schema: "identity",
                table: "Receivables",
                column: "ContactId",
                principalSchema: "identity",
                principalTable: "Contacts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SpotPayables_Contacts_ContactId",
                schema: "identity",
                table: "SpotPayables",
                column: "ContactId",
                principalSchema: "identity",
                principalTable: "Contacts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payables_Contacts_ContactId",
                schema: "identity",
                table: "Payables");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_Contacts_ContactId",
                schema: "identity",
                table: "Quotations");

            migrationBuilder.DropForeignKey(
                name: "FK_Receivables_Contacts_ContactId",
                schema: "identity",
                table: "Receivables");

            migrationBuilder.DropForeignKey(
                name: "FK_SpotPayables_Contacts_ContactId",
                schema: "identity",
                table: "SpotPayables");

            migrationBuilder.DropIndex(
                name: "IX_SpotPayables_InvoiceId",
                schema: "identity",
                table: "SpotPayables");

            migrationBuilder.DropIndex(
                name: "IX_Receivables_InvoiceId",
                schema: "identity",
                table: "Receivables");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_InvoiceId",
                schema: "identity",
                table: "Quotations");

            migrationBuilder.AlterColumn<long>(
                name: "InvoiceId",
                schema: "identity",
                table: "SpotPayables",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ContactId",
                schema: "identity",
                table: "SpotPayables",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "InvoiceId",
                schema: "identity",
                table: "Receivables",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ContactId",
                schema: "identity",
                table: "Receivables",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "InvoiceId",
                schema: "identity",
                table: "Quotations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ContactId",
                schema: "identity",
                table: "Quotations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ContactId",
                schema: "identity",
                table: "Payables",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CustomerId",
                schema: "identity",
                table: "Invoices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpotPayables_InvoiceId",
                schema: "identity",
                table: "SpotPayables",
                column: "InvoiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receivables_InvoiceId",
                schema: "identity",
                table: "Receivables",
                column: "InvoiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_InvoiceId",
                schema: "identity",
                table: "Quotations",
                column: "InvoiceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payables_Contacts_ContactId",
                schema: "identity",
                table: "Payables",
                column: "ContactId",
                principalSchema: "identity",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_Contacts_ContactId",
                schema: "identity",
                table: "Quotations",
                column: "ContactId",
                principalSchema: "identity",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receivables_Contacts_ContactId",
                schema: "identity",
                table: "Receivables",
                column: "ContactId",
                principalSchema: "identity",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SpotPayables_Contacts_ContactId",
                schema: "identity",
                table: "SpotPayables",
                column: "ContactId",
                principalSchema: "identity",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
