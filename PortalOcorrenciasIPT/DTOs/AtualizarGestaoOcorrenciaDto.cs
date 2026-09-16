using System.ComponentModel.DataAnnotations;

namespace PortalOcorrenciasIPT.DTOs;

// DTO usado pela API para atualizar os campos de gestão de uma ocorrência.
// Apenas contém Estado e Prioridade, evitando expor todos os campos do modelo Ocorrencia.
public class AtualizarGestaoOcorrenciaDto
{
    [Required(ErrorMessage = "O estado é obrigatório.")]
    public string Estado { get; set; } = "";

    [Required(ErrorMessage = "A prioridade é obrigatória.")]
    public string Prioridade { get; set; } = "";
}