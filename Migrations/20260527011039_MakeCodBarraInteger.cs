using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minipdv.Migrations
{
    /// <inheritdoc />
    public partial class MakeCodBarraInteger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE Produtos
                SET CodBarra = '0'
                WHERE CodBarra IS NULL OR TRY_CAST(CodBarra AS int) IS NULL");

            migrationBuilder.Sql(@"
                DECLARE @constraint sysname = (
                    SELECT name FROM sys.default_constraints
                    WHERE parent_object_id = OBJECT_ID('Produtos')
                    AND parent_column_id = COLUMNPROPERTY(OBJECT_ID('Produtos'), 'CodBarra', 'ColumnId')
                );
                IF @constraint IS NOT NULL
                    EXEC('ALTER TABLE Produtos DROP CONSTRAINT ' + @constraint)");

            migrationBuilder.Sql(@"
                ALTER TABLE Produtos
                ALTER COLUMN CodBarra int NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE Produtos
                ALTER COLUMN CodBarra nvarchar(14) NOT NULL");
        }
    }
}
