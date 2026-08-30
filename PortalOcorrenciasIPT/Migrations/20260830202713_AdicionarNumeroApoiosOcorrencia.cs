using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortalOcorrenciasIPT.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarNumeroApoiosOcorrencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumeroApoios",
                table: "Ocorrencias",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroApoios",
                table: "Ocorrencias");
        }
    }
}
