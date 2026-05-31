using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minipdv.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProdutoEstoqueFKFromVendaProdutoEstoque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendaProdutoEstoque_ProdutoEstoques_ProdutoId_Lote",
                table: "VendaProdutoEstoque");

            migrationBuilder.DropIndex(
                name: "IX_VendaProdutoEstoque_ProdutoId_Lote",
                table: "VendaProdutoEstoque");

            migrationBuilder.AlterColumn<string>(
                name: "Lote",
                table: "VendaProdutoEstoque",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Lote",
                table: "VendaProdutoEstoque",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_VendaProdutoEstoque_ProdutoId_Lote",
                table: "VendaProdutoEstoque",
                columns: new[] { "ProdutoId", "Lote" });

            migrationBuilder.AddForeignKey(
                name: "FK_VendaProdutoEstoque_ProdutoEstoques_ProdutoId_Lote",
                table: "VendaProdutoEstoque",
                columns: new[] { "ProdutoId", "Lote" },
                principalTable: "ProdutoEstoques",
                principalColumns: new[] { "ProdutoId", "Lote" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
