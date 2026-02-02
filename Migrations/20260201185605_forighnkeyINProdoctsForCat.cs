using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WallsShop.Migrations
{
    /// <inheritdoc />
    public partial class forighnkeyINProdoctsForCat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryFK",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryFK",
                table: "Products",
                column: "CategoryFK");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CategoryImages_CategoryFK",
                table: "Products",
                column: "CategoryFK",
                principalTable: "CategoryImages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CategoryImages_CategoryFK",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryFK",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryFK",
                table: "Products");
        }
    }
}
