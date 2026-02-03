using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WallsShop.Migrations
{
    /// <inheritdoc />
    public partial class forighnkeyINReviewsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserFK",
                table: "Reviews",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserFK",
                table: "Reviews",
                column: "UserFK");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_UserFK",
                table: "Reviews",
                column: "UserFK",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_UserFK",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_UserFK",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UserFK",
                table: "Reviews");
        }
    }
}
