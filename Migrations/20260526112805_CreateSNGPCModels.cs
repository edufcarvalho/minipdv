using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minipdv.Migrations
{
    /// <inheritdoc />
    public partial class CreateSNGPCModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TipoUsuario",
                table: "Usuarios",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);

            migrationBuilder.AddColumn<int>(
                name: "ContatoId",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClasseTerapeutica",
                table: "Produtos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodBarra",
                table: "Produtos",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Controlado",
                table: "Produtos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Dosagem",
                table: "Produtos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FabricanteId",
                table: "Produtos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lista",
                table: "Produtos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrincipioAtivoId",
                table: "Produtos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RegistroMS",
                table: "Produtos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Contatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contatos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrincipiosAtivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrincipiosAtivos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProdutoTipos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoTipos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fabricantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeFantasia = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RazaoSocial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    ContatoId = table.Column<int>(type: "int", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fabricantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fabricantes_Contatos_ContatoId",
                        column: x => x.ContatoId,
                        principalTable: "Contatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_ContatoId",
                table: "Usuarios",
                column: "ContatoId",
                unique: true,
                filter: "[ContatoId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_FabricanteId",
                table: "Produtos",
                column: "FabricanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_PrincipioAtivoId",
                table: "Produtos",
                column: "PrincipioAtivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Fabricantes_ContatoId",
                table: "Fabricantes",
                column: "ContatoId",
                unique: true,
                filter: "[ContatoId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Fabricantes_FabricanteId",
                table: "Produtos",
                column: "FabricanteId",
                principalTable: "Fabricantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_PrincipiosAtivos_PrincipioAtivoId",
                table: "Produtos",
                column: "PrincipioAtivoId",
                principalTable: "PrincipiosAtivos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_Produtos_Fabricantes_FabricanteId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_PrincipiosAtivos_PrincipioAtivoId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Contatos_ContatoId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "Fabricantes");

            migrationBuilder.DropTable(
                name: "PrincipiosAtivos");

            migrationBuilder.DropTable(
                name: "ProdutoTipos");

            migrationBuilder.DropTable(
                name: "Contatos");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_ContatoId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_FabricanteId",
                table: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_PrincipioAtivoId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "ContatoId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ClasseTerapeutica",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "CodBarra",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Controlado",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Dosagem",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "FabricanteId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Lista",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "PrincipioAtivoId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "RegistroMS",
                table: "Produtos");

            migrationBuilder.AlterColumn<string>(
                name: "TipoUsuario",
                table: "Usuarios",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(21)",
                oldMaxLength: 21);
        }
    }
}
