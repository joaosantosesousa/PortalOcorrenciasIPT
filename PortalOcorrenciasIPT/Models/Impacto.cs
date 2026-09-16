using System.ComponentModel.DataAnnotations;

namespace PortalOcorrenciasIPT.Models;

// Representa um tipo de impacto que pode ser associado a uma ocorrência,
// por exemplo "Impede aulas", "Risco de segurança" ou "Acesso bloqueado".
public class Impacto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do impacto é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome do impacto não pode exceder 100 caracteres.")]
    public string Nome { get; set; } = "";

    [StringLength(250, ErrorMessage = "A descrição não pode exceder 250 caracteres.")]
    public string? Descricao { get; set; }

    // Permite esconder impactos que já não devam ser usados em novas ocorrências,
    // sem apagar o histórico das ocorrências onde tenham sido selecionados.
    public bool Ativo { get; set; } = true;

    // Relação muitos-para-muitos com Ocorrencia,
    // implementada através da tabela de junção OcorrenciaImpacto.
    public List<OcorrenciaImpacto> OcorrenciaImpactos { get; set; } = new();
}