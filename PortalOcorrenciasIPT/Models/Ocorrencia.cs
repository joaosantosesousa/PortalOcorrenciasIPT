using System.ComponentModel.DataAnnotations;

namespace PortalOcorrenciasIPT.Models;

public class Ocorrencia
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O título da ocorrência é obrigatório.")]
    [StringLength(100, ErrorMessage = "O título não pode exceder 100 caracteres.")]
    public string Titulo { get; set; } = "";

    [Required(ErrorMessage = "A descrição da ocorrência é obrigatória.")]
    [MinLength(10, ErrorMessage = "A descrição deve ter pelo menos 10 caracteres.")]
    [StringLength(1000, ErrorMessage = "A descrição não pode exceder 1000 caracteres.")]
    public string Descricao { get; set; } = "";

    [Required(ErrorMessage = "A localização é obrigatória.")]
    [StringLength(150, ErrorMessage = "A localização não pode exceder 150 caracteres.")]
    public string LocalizacaoTexto { get; set; } = "";

    [StringLength(80, ErrorMessage = "O edifício não pode exceder 80 caracteres.")]
    public string? Edificio { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    [Required]
    public string Estado { get; set; } = "Aberta";

    [Required]
    public string Prioridade { get; set; } = "Normal";

    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria.")]
    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }
}