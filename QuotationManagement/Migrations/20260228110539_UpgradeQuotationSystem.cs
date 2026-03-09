using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuotationManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeQuotationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Quotations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Quotations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LeadId",
                table: "Quotations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentQuotationId",
                table: "Quotations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Quotations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "QuotationLineItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "LeadId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "ParentQuotationId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "QuotationLineItems");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Quotations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
