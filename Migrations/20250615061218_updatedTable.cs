using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesforceIntegrationApp.Migrations
{
    /// <inheritdoc />
    public partial class updatedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Leads",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Leads");
        }
    }
}
