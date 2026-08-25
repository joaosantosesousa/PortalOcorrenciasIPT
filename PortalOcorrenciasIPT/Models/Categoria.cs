using System.ComponentModel.DataAnnotations;

namespace PortalOcorrenciasIPT.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
    [StringLength(80, ErrorMessage = "O nome da categoria não pode exceder 80 caracteres.")]
    public string Nome { get; set; } = "";

    [StringLength(250, ErrorMessage = "A descrição não pode exceder 250 caracteres.")]
    public string? Descricao { get; set; }

    public bool Ativa { get; set; } = true;
    public List<Ocorrencia> Ocorrencias { get; set; } = new();
}