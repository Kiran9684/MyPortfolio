using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPortfolio.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ProfilePictureTableCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "userProfilePicture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageType = table.Column<string>(type: "varchar(25)", nullable: true),
                    PublicImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userProfilePicture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userProfilePicture_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_userProfilePicture_PublicImageId",
                table: "userProfilePicture",
                column: "PublicImageId",
                unique: true,
                filter: "[PublicImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_userProfilePicture_UserId",
                table: "userProfilePicture",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userProfilePicture");
        }
    }
}
