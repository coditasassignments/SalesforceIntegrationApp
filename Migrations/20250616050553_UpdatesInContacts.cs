using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesforceIntegrationApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesInContacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Contacts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Contacts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
