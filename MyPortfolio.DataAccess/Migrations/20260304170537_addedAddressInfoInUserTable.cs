using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPortfolio.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addedAddressInfoInUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address1",
                table: "user",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "user",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "user",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "user",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "user",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address1",
                table: "user");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "user");

            migrationBuilder.DropColumn(
                name: "City",
                table: "user");

            migrationBuilder.DropColumn(
                name: "State",
                table: "user");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "user");
        }
    }
}
