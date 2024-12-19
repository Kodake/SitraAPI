using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class RemovedBiller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receivables_Contacts_BillerId",
                table: "Receivables");

            migrationBuilder.DropForeignKey(
                name: "FK_Receivables_Contacts_BuyerId",
                table: "Receivables");

            migrationBuilder.DropIndex(
                name: "IX_Receivables_BillerId",
                table: "Receivables");

            migrationBuilder.DropColumn(
                name: "BillerId",
                table: "Receivables");

            migrationBuilder.RenameColumn(
                name: "BuyerId",
                table: "Receivables",
                newName: "ContactId");

            migrationBuilder.RenameIndex(
                name: "IX_Receivables_BuyerId",
                table: "Receivables",
                newName: "IX_Receivables_ContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receivables_Contacts_ContactId",
                table: "Receivables",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receivables_Contacts_ContactId",
                table: "Receivables");

            migrationBuilder.RenameColumn(
                name: "ContactId",
                table: "Receivables",
                newName: "BuyerId");

            migrationBuilder.RenameIndex(
                name: "IX_Receivables_ContactId",
                table: "Receivables",
                newName: "IX_Receivables_BuyerId");

            migrationBuilder.AddColumn<long>(
                name: "BillerId",
                table: "Receivables",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Receivables_BillerId",
                table: "Receivables",
                column: "BillerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receivables_Contacts_BillerId",
                table: "Receivables",
                column: "BillerId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Receivables_Contacts_BuyerId",
                table: "Receivables",
                column: "BuyerId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
