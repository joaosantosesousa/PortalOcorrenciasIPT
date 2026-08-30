using System.ComponentModel.DataAnnotations;

namespace PortalOcorrenciasIPT.Models;

public class Impacto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do impacto é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome do impacto não pode exceder 100 caracteres.")]
    public string Nome { get; set; } = "";

    [StringLength(250, ErrorMessage = "A descrição não pode exceder 250 caracteres.")]
    public string? Descricao { get; set; }

    public bool Ativo { get; set; } = true;

    public List<OcorrenciaImpacto> OcorrenciaImpactos { get; set; } = new();
}