using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class SolvedSchemaForTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Warehouses",
                schema: "identity",
                newName: "Warehouses");

            migrationBuilder.RenameTable(
                name: "Units",
                schema: "identity",
                newName: "Units");

            migrationBuilder.RenameTable(
                name: "Transfers",
                schema: "identity",
                newName: "Transfers");

            migrationBuilder.RenameTable(
                name: "Taxes",
                schema: "identity",
                newName: "Taxes");

            migrationBuilder.RenameTable(
                name: "SubCategories",
                schema: "identity",
                newName: "SubCategories");

            migrationBuilder.RenameTable(
                name: "Stocks",
                schema: "identity",
                newName: "Stocks");

            migrationBuilder.RenameTable(
                name: "SpotPayments",
                schema: "identity",
                newName: "SpotPayments");

            migrationBuilder.RenameTable(
                name: "SpotPayables",
                schema: "identity",
                newName: "SpotPayables");

            migrationBuilder.RenameTable(
                name: "Receivables",
                schema: "identity",
                newName: "Receivables");

            migrationBuilder.RenameTable(
                name: "Quotations",
                schema: "identity",
                newName: "Quotations");

            migrationBuilder.RenameTable(
                name: "Pricings",
                schema: "identity",
                newName: "Pricings");

            migrationBuilder.RenameTable(
                name: "PricingItems",
                schema: "identity",
                newName: "PricingItems");

            migrationBuilder.RenameTable(
                name: "PettyCashMovements",
                schema: "identity",
                newName: "PettyCashMovements");

            migrationBuilder.RenameTable(
                name: "PettyCashes",
                schema: "identity",
                newName: "PettyCashes");

            migrationBuilder.RenameTable(
                name: "Payments",
                schema: "identity",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "PaymentReceipts",
                schema: "identity",
                newName: "PaymentReceipts");

            migrationBuilder.RenameTable(
                name: "Payables",
                schema: "identity",
                newName: "Payables");

            migrationBuilder.RenameTable(
                name: "PayableReceipts",
                schema: "identity",
                newName: "PayableReceipts");

            migrationBuilder.RenameTable(
                name: "Models",
                schema: "identity",
                newName: "Models");

            migrationBuilder.RenameTable(
                name: "ItemTaxes",
                schema: "identity",
                newName: "ItemTaxes");

            migrationBuilder.RenameTable(
                name: "Items",
                schema: "identity",
                newName: "Items");

            migrationBuilder.RenameTable(
                name: "Invoices",
                schema: "identity",
                newName: "Invoices");

            migrationBuilder.RenameTable(
                name: "InvoiceItems",
                schema: "identity",
                newName: "InvoiceItems");

            migrationBuilder.RenameTable(
                name: "FiscalSupports",
                schema: "identity",
                newName: "FiscalSupports");

            migrationBuilder.RenameTable(
                name: "Expenses",
                schema: "identity",
                newName: "Expenses");

            migrationBuilder.RenameTable(
                name: "Deadlines",
                schema: "identity",
                newName: "Deadlines");

            migrationBuilder.RenameTable(
                name: "Currencies",
                schema: "identity",
                newName: "Currencies");

            migrationBuilder.RenameTable(
                name: "Contacts",
                schema: "identity",
                newName: "Contacts");

            migrationBuilder.RenameTable(
                name: "ComboItems",
                schema: "identity",
                newName: "ComboItems");

            migrationBuilder.RenameTable(
                name: "Categories",
                schema: "identity",
                newName: "Categories");

            migrationBuilder.RenameTable(
                name: "Brands",
                schema: "identity",
                newName: "Brands");

            migrationBuilder.RenameTable(
                name: "Appointments",
                schema: "identity",
                newName: "Appointments");

            migrationBuilder.RenameTable(
                name: "AppointmentItems",
                schema: "identity",
                newName: "AppointmentItems");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                schema: "identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RucNumber",
                schema: "identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                schema: "identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                schema: "identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "identity",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Warehouses",
                newName: "Warehouses",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Units",
                newName: "Units",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Transfers",
                newName: "Transfers",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Taxes",
                newName: "Taxes",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "SubCategories",
                newName: "SubCategories",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Stocks",
                newName: "Stocks",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "SpotPayments",
                newName: "SpotPayments",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "SpotPayables",
                newName: "SpotPayables",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Receivables",
                newName: "Receivables",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Quotations",
                newName: "Quotations",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Pricings",
                newName: "Pricings",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "PricingItems",
                newName: "PricingItems",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "PettyCashMovements",
                newName: "PettyCashMovements",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "PettyCashes",
                newName: "PettyCashes",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payments",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "PaymentReceipts",
                newName: "PaymentReceipts",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Payables",
                newName: "Payables",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "PayableReceipts",
                newName: "PayableReceipts",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Models",
                newName: "Models",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "ItemTaxes",
                newName: "ItemTaxes",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Items",
                newName: "Items",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newName: "Invoices",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "InvoiceItems",
                newName: "InvoiceItems",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "FiscalSupports",
                newName: "FiscalSupports",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Expenses",
                newName: "Expenses",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Deadlines",
                newName: "Deadlines",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Currencies",
                newName: "Currencies",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Contacts",
                newName: "Contacts",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "ComboItems",
                newName: "ComboItems",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Categories",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Brands",
                newName: "Brands",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Appointments",
                newName: "Appointments",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AppointmentItems",
                newName: "AppointmentItems",
                newSchema: "identity");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                schema: "identity",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RucNumber",
                schema: "identity",
                table: "AspNetUsers",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                schema: "identity",
                table: "AspNetUsers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                schema: "identity",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                schema: "identity",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
