using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class RemovedDeadlineForSaleInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleInvoices_Deadlines_DeadlineId",
                table: "SaleInvoices");

            migrationBuilder.DropIndex(
                name: "IX_SaleInvoices_DeadlineId",
                table: "SaleInvoices");

            migrationBuilder.DropColumn(
                name: "DeadlineId",
                table: "SaleInvoices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DeadlineId",
                table: "SaleInvoices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_SaleInvoices_DeadlineId",
                table: "SaleInvoices",
                column: "DeadlineId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleInvoices_Deadlines_DeadlineId",
                table: "SaleInvoices",
                column: "DeadlineId",
                principalTable: "Deadlines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
