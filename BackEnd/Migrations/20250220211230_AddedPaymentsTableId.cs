using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddedPaymentsTableId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_SpotPayments_SpotPaymentId",
                table: "Payments");

            migrationBuilder.AlterColumn<long>(
                name: "SpotPaymentId",
                table: "Payments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_SpotPayments_SpotPaymentId",
                table: "Payments",
                column: "SpotPaymentId",
                principalTable: "SpotPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_SpotPayments_SpotPaymentId",
                table: "Payments");

            migrationBuilder.AlterColumn<long>(
                name: "SpotPaymentId",
                table: "Payments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_SpotPayments_SpotPaymentId",
                table: "Payments",
                column: "SpotPaymentId",
                principalTable: "SpotPayments",
                principalColumn: "Id");
        }
    }
}
