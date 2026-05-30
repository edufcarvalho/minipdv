using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minipdv.Migrations
{
    /// <inheritdoc />
    public partial class CreateReceitaAndLinkTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Receitas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescritorId = table.Column<int>(type: "int", nullable: false),
                    PacienteId = table.Column<int>(type: "int", nullable: false),
                    CompradorId = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receitas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receitas_Clientes_CompradorId",
                        column: x => x.CompradorId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Receitas_Clientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Receitas_Prescritores_PrescritorId",
                        column: x => x.PrescritorId,
                        principalTable: "Prescritores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReceitaProdutoEstoque",
                columns: table => new
                {
                    ReceitaId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    Lote = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceitaProdutoEstoque", x => new { x.ReceitaId, x.ProdutoId, x.Lote });
                    table.ForeignKey(
                        name: "FK_ReceitaProdutoEstoque_ProdutoEstoques_ProdutoId_Lote",
                        columns: x => new { x.ProdutoId, x.Lote },
                        principalTable: "ProdutoEstoques",
                        principalColumns: new[] { "ProdutoId", "Lote" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceitaProdutoEstoque_Receitas_ReceitaId",
                        column: x => x.ReceitaId,
                        principalTable: "Receitas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceitaProdutoEstoque_ProdutoId_Lote",
                table: "ReceitaProdutoEstoque",
                columns: new[] { "ProdutoId", "Lote" });

            migrationBuilder.CreateIndex(
                name: "IX_Receitas_CompradorId",
                table: "Receitas",
                column: "CompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Receitas_PacienteId",
                table: "Receitas",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Receitas_PrescritorId",
                table: "Receitas",
                column: "PrescritorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceitaProdutoEstoque");

            migrationBuilder.DropTable(
                name: "Receitas");
        }
    }
}
