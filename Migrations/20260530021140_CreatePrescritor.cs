using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minipdv.Migrations
{
    /// <inheritdoc />
    public partial class CreatePrescritor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prescritores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Conselho = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Uf = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescritores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prescritores_Numero_Conselho_Uf",
                table: "Prescritores",
                columns: new[] { "Numero", "Conselho", "Uf" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prescritores");
        }
    }
}
