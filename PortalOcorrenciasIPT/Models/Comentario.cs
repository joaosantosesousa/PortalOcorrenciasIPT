using System.ComponentModel.DataAnnotations;
using PortalOcorrenciasIPT.Data;

namespace PortalOcorrenciasIPT.Models;

public class Comentario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O comentário é obrigatório.")]
    [MinLength(3, ErrorMessage = "O comentário deve ter pelo menos 3 caracteres.")]
    [StringLength(1000, ErrorMessage = "O comentário não pode exceder 1000 caracteres.")]
    public string Texto { get; set; } = "";

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public int OcorrenciaId { get; set; }

    public Ocorrencia? Ocorrencia { get; set; }

    public string UtilizadorId { get; set; } = "";

    public ApplicationUser? Utilizador { get; set; }
}