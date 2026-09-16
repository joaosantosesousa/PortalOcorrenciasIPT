using System.ComponentModel.DataAnnotations;
using PortalOcorrenciasIPT.Data;

namespace PortalOcorrenciasIPT.Models;

// Entidade principal da aplicação.
// Representa uma ocorrência registada no campus do IPT.
public class Ocorrencia
{
    public int Id { get; set; }

    // Dados principais introduzidos pelo utilizador ao criar uma ocorrência.
    [Required(ErrorMessage = "O título da ocorrência é obrigatório.")]
    [StringLength(100, ErrorMessage = "O título não pode exceder 100 caracteres.")]
    public string Titulo { get; set; } = "";

    [Required(ErrorMessage = "A descrição da ocorrência é obrigatória.")]
    [MinLength(10, ErrorMessage = "A descrição deve ter pelo menos 10 caracteres.")]
    [StringLength(1000, ErrorMessage = "A descrição não pode exceder 1000 caracteres.")]
    public string Descricao { get; set; } = "";

    // Localização textual da ocorrência. 
    // A opção por texto permite identificar salas, edifícios ou zonas específicas do campus.
    [Required(ErrorMessage = "A localização é obrigatória.")]
    [StringLength(150, ErrorMessage = "A localização não pode exceder 150 caracteres.")]
    public string LocalizacaoTexto { get; set; } = "";

    [StringLength(80, ErrorMessage = "O edifício não pode exceder 80 caracteres.")]
    public string? Edificio { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    // Estado e prioridade são usados na gestão da ocorrência pelo perfil Gestor.
    [Required]
    public string Estado { get; set; } = "Aberta";

    [Required]
    public string Prioridade { get; set; } = "Normal";

    // Contador público da opção "Também sou afetado".
    // Não exige autenticação e permite medir o impacto percecionado pela comunidade.
    public int NumeroApoios { get; set; } = 0;

    // Relação muitos-para-um: uma Categoria pode ter várias Ocorrencias,
    // mas cada Ocorrencia pertence a uma única Categoria.
    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria.")]
    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }

    // Associação opcional ao utilizador autenticado que criou a ocorrência.
    // É opcional porque, se o utilizador for removido, a ocorrência é preservada.
    public string? UtilizadorId { get; set; }

    public ApplicationUser? Utilizador { get; set; }

    // Relação muitos-para-muitos com Impacto,
    // implementada através da tabela de junção OcorrenciaImpacto.
    public List<OcorrenciaImpacto> OcorrenciaImpactos { get; set; } = new();

    // Relação um-para-muitos: uma ocorrência pode ter vários comentários.
    public List<Comentario> Comentarios { get; set; } = new();
}