using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minipdv.Migrations
{
    /// <inheritdoc />
    public partial class CreateProdutoGrupo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProdutoGrupoId",
                table: "Produtos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProdutoGrupos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoGrupos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_ProdutoGrupoId",
                table: "Produtos",
                column: "ProdutoGrupoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_ProdutoGrupos_ProdutoGrupoId",
                table: "Produtos",
                column: "ProdutoGrupoId",
                principalTable: "ProdutoGrupos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_ProdutoGrupos_ProdutoGrupoId",
                table: "Produtos");

            migrationBuilder.DropTable(
                name: "ProdutoGrupos");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_ProdutoGrupoId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "ProdutoGrupoId",
                table: "Produtos");
        }
    }
}
