using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SITRA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedCertificateInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CertificatePassword",
                schema: "auths",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificateUrl",
                schema: "auths",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificatePassword",
                schema: "auths",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CertificateUrl",
                schema: "auths",
                table: "AspNetUsers");
        }
    }
}
