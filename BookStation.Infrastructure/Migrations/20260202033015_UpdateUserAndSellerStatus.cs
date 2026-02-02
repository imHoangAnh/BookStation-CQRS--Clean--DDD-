using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserAndSellerStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "SellerProfiles");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SellerProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "SellerProfiles");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "SellerProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
