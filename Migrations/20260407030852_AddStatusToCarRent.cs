using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LUXURY_DRIVE.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToCarRent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CarRents",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "CarRents");
        }
    }
}
