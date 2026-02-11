using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SITRA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddexIX : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_TenantId",
                table: "Warehouses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_TenantId",
                table: "Units",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_TenantId",
                table: "Transfers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_TenantId",
                table: "Taxes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_TenantId",
                table: "SubCategories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_TenantId",
                table: "Stocks",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SpotPayments_TenantId",
                table: "SpotPayments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SpotPayables_TenantId",
                table: "SpotPayables",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Receivables_TenantId",
                table: "Receivables",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_TenantId",
                table: "Quotations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Pricings_TenantId",
                table: "Pricings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PricingItems_TenantId",
                table: "PricingItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashMovements_TenantId",
                table: "PettyCashMovements",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashes_TenantId",
                table: "PettyCashes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TenantId",
                table: "Payments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentReceipts_TenantId",
                table: "PaymentReceipts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Payables_TenantId",
                table: "Payables",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PayableReceipts_TenantId",
                table: "PayableReceipts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_TenantId",
                table: "Models",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemUnits_TenantId",
                table: "ItemUnits",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTaxes_TenantId",
                table: "ItemTaxes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSubCategories_TenantId",
                table: "ItemSubCategories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_TenantId",
                table: "Items",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemModels_TenantId",
                table: "ItemModels",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCategories_TenantId",
                table: "ItemCategories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemBrands_TenantId",
                table: "ItemBrands",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_TenantId",
                table: "Invoices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_TenantId",
                table: "InvoiceItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceFiscalSupports_TenantId",
                table: "InvoiceFiscalSupports",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceBuyers_TenantId",
                table: "InvoiceBuyers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceBillers_TenantId",
                table: "InvoiceBillers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FiscalSupports_TenantId",
                table: "FiscalSupports",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TenantId",
                table: "Expenses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Deadlines_TenantId",
                table: "Deadlines",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_TenantId",
                table: "Currencies",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_TenantId",
                table: "Contacts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboItems_TenantId",
                table: "ComboItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenantId",
                table: "Categories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_TenantId",
                table: "Brands",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentItems_TenantId",
                table: "AppointmentItems",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Warehouses_TenantId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Units_TenantId",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_TenantId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Taxes_TenantId",
                table: "Taxes");

            migrationBuilder.DropIndex(
                name: "IX_SubCategories_TenantId",
                table: "SubCategories");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_TenantId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_SpotPayments_TenantId",
                table: "SpotPayments");

            migrationBuilder.DropIndex(
                name: "IX_SpotPayables_TenantId",
                table: "SpotPayables");

            migrationBuilder.DropIndex(
                name: "IX_Receivables_TenantId",
                table: "Receivables");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_TenantId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Pricings_TenantId",
                table: "Pricings");

            migrationBuilder.DropIndex(
                name: "IX_PricingItems_TenantId",
                table: "PricingItems");

            migrationBuilder.DropIndex(
                name: "IX_PettyCashMovements_TenantId",
                table: "PettyCashMovements");

            migrationBuilder.DropIndex(
                name: "IX_PettyCashes_TenantId",
                table: "PettyCashes");

            migrationBuilder.DropIndex(
                name: "IX_Payments_TenantId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_PaymentReceipts_TenantId",
                table: "PaymentReceipts");

            migrationBuilder.DropIndex(
                name: "IX_Payables_TenantId",
                table: "Payables");

            migrationBuilder.DropIndex(
                name: "IX_PayableReceipts_TenantId",
                table: "PayableReceipts");

            migrationBuilder.DropIndex(
                name: "IX_Models_TenantId",
                table: "Models");

            migrationBuilder.DropIndex(
                name: "IX_ItemUnits_TenantId",
                table: "ItemUnits");

            migrationBuilder.DropIndex(
                name: "IX_ItemTaxes_TenantId",
                table: "ItemTaxes");

            migrationBuilder.DropIndex(
                name: "IX_ItemSubCategories_TenantId",
                table: "ItemSubCategories");

            migrationBuilder.DropIndex(
                name: "IX_Items_TenantId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_ItemModels_TenantId",
                table: "ItemModels");

            migrationBuilder.DropIndex(
                name: "IX_ItemCategories_TenantId",
                table: "ItemCategories");

            migrationBuilder.DropIndex(
                name: "IX_ItemBrands_TenantId",
                table: "ItemBrands");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_TenantId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_TenantId",
                table: "InvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceFiscalSupports_TenantId",
                table: "InvoiceFiscalSupports");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceBuyers_TenantId",
                table: "InvoiceBuyers");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceBillers_TenantId",
                table: "InvoiceBillers");

            migrationBuilder.DropIndex(
                name: "IX_FiscalSupports_TenantId",
                table: "FiscalSupports");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_TenantId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Deadlines_TenantId",
                table: "Deadlines");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_TenantId",
                table: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_TenantId",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_ComboItems_TenantId",
                table: "ComboItems");

            migrationBuilder.DropIndex(
                name: "IX_Categories_TenantId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Brands_TenantId",
                table: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentItems_TenantId",
                table: "AppointmentItems");
        }
    }
}
