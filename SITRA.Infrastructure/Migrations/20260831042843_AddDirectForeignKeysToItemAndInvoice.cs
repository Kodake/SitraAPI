using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SITRA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDirectForeignKeysToItemAndInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceBillers");

            migrationBuilder.DropTable(
                name: "InvoiceBuyers");

            migrationBuilder.DropTable(
                name: "InvoiceFiscalSupports");

            migrationBuilder.DropTable(
                name: "ItemBrands");

            migrationBuilder.DropTable(
                name: "ItemCategories");

            migrationBuilder.DropTable(
                name: "ItemModels");

            migrationBuilder.DropTable(
                name: "ItemSubCategories");

            migrationBuilder.AddColumn<Guid>(
                name: "BrandId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModelId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SubCategoryId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BillerId",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BuyerId",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FiscalSupportId",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_BrandId",
                table: "Items",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryId",
                table: "Items",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ModelId",
                table: "Items",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_SubCategoryId",
                table: "Items",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BillerId",
                table: "Invoices",
                column: "BillerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BuyerId",
                table: "Invoices",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_FiscalSupportId",
                table: "Invoices",
                column: "FiscalSupportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Contacts_BillerId",
                table: "Invoices",
                column: "BillerId",
                principalTable: "Contacts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Contacts_BuyerId",
                table: "Invoices",
                column: "BuyerId",
                principalTable: "Contacts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_FiscalSupports_FiscalSupportId",
                table: "Invoices",
                column: "FiscalSupportId",
                principalTable: "FiscalSupports",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Brands_BrandId",
                table: "Items",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Models_ModelId",
                table: "Items",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_SubCategories_SubCategoryId",
                table: "Items",
                column: "SubCategoryId",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Contacts_BillerId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Contacts_BuyerId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_FiscalSupports_FiscalSupportId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Brands_BrandId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Models_ModelId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_SubCategories_SubCategoryId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_BrandId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CategoryId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_ModelId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_SubCategoryId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_BillerId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_BuyerId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_FiscalSupportId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BillerId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "FiscalSupportId",
                table: "Invoices");

            migrationBuilder.CreateTable(
                name: "InvoiceBillers",
                columns: table => new
                {
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceBillers", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_InvoiceBillers_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceBillers_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceBuyers",
                columns: table => new
                {
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceBuyers", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_InvoiceBuyers_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceBuyers_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceFiscalSupports",
                columns: table => new
                {
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FiscalSupportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EcfType = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceFiscalSupports", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_InvoiceFiscalSupports_FiscalSupports_FiscalSupportId",
                        column: x => x.FiscalSupportId,
                        principalTable: "FiscalSupports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceFiscalSupports_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemBrands",
                columns: table => new
                {
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemBrands", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_ItemBrands_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemBrands_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemCategories",
                columns: table => new
                {
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCategories", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_ItemCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemCategories_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemModels",
                columns: table => new
                {
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemModels", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_ItemModels_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemModels_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemSubCategories",
                columns: table => new
                {
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSubCategories", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_ItemSubCategories_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemSubCategories_SubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceBillers_ContactId",
                table: "InvoiceBillers",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceBillers_TenantId",
                table: "InvoiceBillers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceBuyers_ContactId",
                table: "InvoiceBuyers",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceBuyers_TenantId",
                table: "InvoiceBuyers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceFiscalSupports_FiscalSupportId",
                table: "InvoiceFiscalSupports",
                column: "FiscalSupportId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceFiscalSupports_TenantId",
                table: "InvoiceFiscalSupports",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemBrands_BrandId",
                table: "ItemBrands",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemBrands_TenantId",
                table: "ItemBrands",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCategories_CategoryId",
                table: "ItemCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCategories_TenantId",
                table: "ItemCategories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemModels_ModelId",
                table: "ItemModels",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemModels_TenantId",
                table: "ItemModels",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSubCategories_SubCategoryId",
                table: "ItemSubCategories",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSubCategories_TenantId",
                table: "ItemSubCategories",
                column: "TenantId");
        }
    }
}
