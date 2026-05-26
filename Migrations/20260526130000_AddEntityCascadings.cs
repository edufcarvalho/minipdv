using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minipdv.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityCascadings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fabricantes_Contatos_ContatoId",
                table: "Fabricantes");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Fabricantes_FabricanteId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_PrincipiosAtivos_PrincipioAtivoId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_ProdutoGrupos_ProdutoGrupoId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Contatos_ContatoId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_ContatoId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Fabricantes_ContatoId",
                table: "Fabricantes");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_ContatoId",
                table: "Usuarios",
                column: "ContatoId",
                unique: true,
                filter: "[ContatoId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Fabricantes_ContatoId",
                table: "Fabricantes",
                column: "ContatoId",
                unique: true,
                filter: "[ContatoId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Fabricantes_Contatos_ContatoId",
                table: "Fabricantes",
                column: "ContatoId",
                principalTable: "Contatos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Fabricantes_FabricanteId",
                table: "Produtos",
                column: "FabricanteId",
                principalTable: "Fabricantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_PrincipiosAtivos_PrincipioAtivoId",
                table: "Produtos",
                column: "PrincipioAtivoId",
                principalTable: "PrincipiosAtivos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_ProdutoGrupos_ProdutoGrupoId",
                table: "Produtos",
                column: "ProdutoGrupoId",
                principalTable: "ProdutoGrupos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Contatos_ContatoId",
                table: "Usuarios",
                column: "ContatoId",
                principalTable: "Contatos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fabricantes_Contatos_ContatoId",
                table: "Fabricantes");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Fabricantes_FabricanteId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_PrincipiosAtivos_PrincipioAtivoId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_ProdutoGrupos_ProdutoGrupoId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Contatos_ContatoId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_ContatoId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Fabricantes_ContatoId",
                table: "Fabricantes");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_ContatoId",
                table: "Usuarios",
                column: "ContatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Fabricantes_ContatoId",
                table: "Fabricantes",
                column: "ContatoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fabricantes_Contatos_ContatoId",
                table: "Fabricantes",
                column: "ContatoId",
                principalTable: "Contatos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Fabricantes_FabricanteId",
                table: "Produtos",
                column: "FabricanteId",
                principalTable: "Fabricantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_PrincipiosAtivos_PrincipioAtivoId",
                table: "Produtos",
                column: "PrincipioAtivoId",
                principalTable: "PrincipiosAtivos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_ProdutoGrupos_ProdutoGrupoId",
                table: "Produtos",
                column: "ProdutoGrupoId",
                principalTable: "ProdutoGrupos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Contatos_ContatoId",
                table: "Usuarios",
                column: "ContatoId",
                principalTable: "Contatos",
                principalColumn: "Id");
        }
    }
}
