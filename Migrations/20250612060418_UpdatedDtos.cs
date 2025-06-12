using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesforceIntegrationApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDtos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "LeadsInProgress",
                newName: "WhoId");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "LeadsInProgress",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Company",
                table: "LeadsInProgress",
                newName: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WhoId",
                table: "LeadsInProgress",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "LeadsInProgress",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "LeadsInProgress",
                newName: "Company");
        }
    }
}
