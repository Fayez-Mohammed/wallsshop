using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WallsShop.Migrations
{
    /// <inheritdoc />
    public partial class fkInOfferscat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryFK",
                table: "Offers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offers_CategoryFK",
                table: "Offers",
                column: "CategoryFK");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_CategoryImages_CategoryFK",
                table: "Offers",
                column: "CategoryFK",
                principalTable: "CategoryImages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_CategoryImages_CategoryFK",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_CategoryFK",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "CategoryFK",
                table: "Offers");
        }
    }
}
