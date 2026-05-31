using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SumX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserTenantIdToGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Users"" ALTER COLUMN ""TenantId"" TYPE uuid USING ""TenantId""::uuid;");
            migrationBuilder.Sql(@"ALTER TABLE ""Tenants"" ALTER COLUMN ""Id"" TYPE uuid USING ""Id""::uuid;");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Users",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(36)",
                oldMaxLength: 36,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Tenants",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(36)",
                oldMaxLength: 36);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Users"" ALTER COLUMN ""TenantId"" TYPE varchar(36) USING ""TenantId""::varchar(36);");
            migrationBuilder.Sql(@"ALTER TABLE ""Tenants"" ALTER COLUMN ""Id"" TYPE varchar(36) USING ""Id""::varchar(36);");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "Users",
                type: "character varying(36)",
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Tenants",
                type: "character varying(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
