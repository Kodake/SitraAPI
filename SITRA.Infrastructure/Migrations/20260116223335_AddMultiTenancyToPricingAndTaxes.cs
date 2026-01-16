using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SITRA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenancyToPricingAndTaxes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Pricings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "PricingItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "ItemTaxes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Pricings");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PricingItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ItemTaxes");
        }
    }
}
