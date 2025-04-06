using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmCapitalDevSrProject.Migrations
{
    /// <inheritdoc />
    public partial class AjustaModeloAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioResponsavel",
                table: "ProdutosAuditoria");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioResponsavel",
                table: "ProdutosAuditoria",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
