using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minipdv.Migrations
{
    /// <inheritdoc />
    public partial class AddPosicaoToVendaItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VendaItem",
                table: "VendaItem");

            migrationBuilder.AddColumn<int>(
                name: "Posicao",
                table: "VendaItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VendaItem",
                table: "VendaItem",
                columns: new[] { "VendaId", "ProdutoId", "Posicao" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VendaItem",
                table: "VendaItem");

            migrationBuilder.DropColumn(
                name: "Posicao",
                table: "VendaItem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VendaItem",
                table: "VendaItem",
                columns: new[] { "VendaId", "ProdutoId" });
        }
    }
}
