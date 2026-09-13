using System.ComponentModel.DataAnnotations;

namespace PortalOcorrenciasIPT.DTOs;

public class AtualizarGestaoOcorrenciaDto
{
    [Required(ErrorMessage = "O estado é obrigatório.")]
    public string Estado { get; set; } = "";

    [Required(ErrorMessage = "A prioridade é obrigatória.")]
    public string Prioridade { get; set; } = "";
}