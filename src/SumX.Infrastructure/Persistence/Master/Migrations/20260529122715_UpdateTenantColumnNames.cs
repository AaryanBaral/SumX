using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SumX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTenantColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Tenants",
                newName: "EmailAddress");

            migrationBuilder.RenameColumn(
                name: "DatabaseConnectionString",
                table: "Tenants",
                newName: "DbConnStr");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmailAddress",
                table: "Tenants",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "DbConnStr",
                table: "Tenants",
                newName: "DatabaseConnectionString");
        }
    }
}
