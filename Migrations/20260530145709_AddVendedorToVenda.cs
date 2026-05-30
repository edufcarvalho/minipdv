using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minipdv.Migrations
{
    /// <inheritdoc />
    public partial class AddVendedorToVenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VendedorId",
                table: "Vendas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_VendedorId",
                table: "Vendas",
                column: "VendedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Usuarios_VendedorId",
                table: "Vendas",
                column: "VendedorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Usuarios_VendedorId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_VendedorId",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "VendedorId",
                table: "Vendas");
        }
    }
}
