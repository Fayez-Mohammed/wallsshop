using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WallsShop.Migrations
{
    /// <inheritdoc />
    public partial class colorsManyTomany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ColorEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnglishColor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewProductColors",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ColorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewProductColors", x => new { x.ProductId, x.ColorId });
                    table.ForeignKey(
                        name: "FK_NewProductColors_ColorEntities_ColorId",
                        column: x => x.ColorId,
                        principalTable: "ColorEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewProductColors_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewProductColors_ColorId",
                table: "NewProductColors",
                column: "ColorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewProductColors");

            migrationBuilder.DropTable(
                name: "ColorEntities");
        }
    }
}
