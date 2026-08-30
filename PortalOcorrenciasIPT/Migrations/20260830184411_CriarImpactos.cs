using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PortalOcorrenciasIPT.Migrations
{
    /// <inheritdoc />
    public partial class CriarImpactos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Impactos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Impactos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OcorrenciaImpactos",
                columns: table => new
                {
                    OcorrenciaId = table.Column<int>(type: "int", nullable: false),
                    ImpactoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcorrenciaImpactos", x => new { x.OcorrenciaId, x.ImpactoId });
                    table.ForeignKey(
                        name: "FK_OcorrenciaImpactos_Impactos_ImpactoId",
                        column: x => x.ImpactoId,
                        principalTable: "Impactos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OcorrenciaImpactos_Ocorrencias_OcorrenciaId",
                        column: x => x.OcorrenciaId,
                        principalTable: "Ocorrencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Impactos",
                columns: new[] { "Id", "Ativo", "Descricao", "Nome" },
                values: new object[,]
                {
                    { 1, true, "A ocorrência tem impacto em mais do que um utilizador ou grupo.", "Afeta várias pessoas" },
                    { 2, true, "A ocorrência impede ou dificulta a realização de aulas.", "Impede aulas" },
                    { 3, true, "A ocorrência pode colocar pessoas ou bens em risco.", "Risco de segurança" },
                    { 4, true, "A ocorrência impede ou dificulta o acesso a um espaço.", "Acesso bloqueado" },
                    { 5, true, "A ocorrência já aconteceu anteriormente.", "Problema recorrente" },
                    { 6, true, "A ocorrência envolve equipamento necessário ao funcionamento normal das atividades.", "Afeta equipamento essencial" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OcorrenciaImpactos_ImpactoId",
                table: "OcorrenciaImpactos",
                column: "ImpactoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OcorrenciaImpactos");

            migrationBuilder.DropTable(
                name: "Impactos");
        }
    }
}
