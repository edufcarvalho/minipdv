using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minipdv.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReceitaIdFromVendaAddCanceladoEmAndVendaIdToReceita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Receitas_ReceitaId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_ReceitaId",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "ReceitaId",
                table: "Vendas");

            migrationBuilder.AddColumn<DateTime>(
                name: "CanceladoEm",
                table: "Vendas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "Sessions",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<int>(
                name: "VendaId",
                table: "Receitas",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Contatos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Contatos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateIndex(
                name: "IX_Receitas_VendaId",
                table: "Receitas",
                column: "VendaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receitas_Vendas_VendaId",
                table: "Receitas",
                column: "VendaId",
                principalTable: "Vendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receitas_Vendas_VendaId",
                table: "Receitas");

            migrationBuilder.DropIndex(
                name: "IX_Receitas_VendaId",
                table: "Receitas");

            migrationBuilder.DropColumn(
                name: "CanceladoEm",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "VendaId",
                table: "Receitas");

            migrationBuilder.AddColumn<int>(
                name: "ReceitaId",
                table: "Vendas",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "Sessions",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048);

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Contatos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Contatos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_ReceitaId",
                table: "Vendas",
                column: "ReceitaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Receitas_ReceitaId",
                table: "Vendas",
                column: "ReceitaId",
                principalTable: "Receitas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
