using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SITRA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDgiiResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DgiiResponse",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DgiiResponse",
                table: "Invoices");
        }
    }
}
