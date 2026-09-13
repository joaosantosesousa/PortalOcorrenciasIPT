using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortalOcorrenciasIPT.Migrations
{
    /// <inheritdoc />
    public partial class AssociarOcorrenciaAUtilizador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UtilizadorId",
                table: "Ocorrencias",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_UtilizadorId",
                table: "Ocorrencias",
                column: "UtilizadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ocorrencias_AspNetUsers_UtilizadorId",
                table: "Ocorrencias",
                column: "UtilizadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ocorrencias_AspNetUsers_UtilizadorId",
                table: "Ocorrencias");

            migrationBuilder.DropIndex(
                name: "IX_Ocorrencias_UtilizadorId",
                table: "Ocorrencias");

            migrationBuilder.DropColumn(
                name: "UtilizadorId",
                table: "Ocorrencias");
        }
    }
}
