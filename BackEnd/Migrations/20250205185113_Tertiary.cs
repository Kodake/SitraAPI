using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class Tertiary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayableReceipt_Payables_PayableId",
                table: "PayableReceipt");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayableReceipt",
                table: "PayableReceipt");

            migrationBuilder.RenameTable(
                name: "PayableReceipt",
                newName: "PayableReceipts");

            migrationBuilder.RenameIndex(
                name: "IX_PayableReceipt_PayableId",
                table: "PayableReceipts",
                newName: "IX_PayableReceipts_PayableId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayableReceipts",
                table: "PayableReceipts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SpotPayables",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<long>(type: "bigint", nullable: false),
                    ContactId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpotPayables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpotPayables_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpotPayables_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpotPayables_ContactId",
                table: "SpotPayables",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_SpotPayables_InvoiceId",
                table: "SpotPayables",
                column: "InvoiceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PayableReceipts_Payables_PayableId",
                table: "PayableReceipts",
                column: "PayableId",
                principalTable: "Payables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayableReceipts_Payables_PayableId",
                table: "PayableReceipts");

            migrationBuilder.DropTable(
                name: "SpotPayables");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayableReceipts",
                table: "PayableReceipts");

            migrationBuilder.RenameTable(
                name: "PayableReceipts",
                newName: "PayableReceipt");

            migrationBuilder.RenameIndex(
                name: "IX_PayableReceipts_PayableId",
                table: "PayableReceipt",
                newName: "IX_PayableReceipt_PayableId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayableReceipt",
                table: "PayableReceipt",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PayableReceipt_Payables_PayableId",
                table: "PayableReceipt",
                column: "PayableId",
                principalTable: "Payables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
