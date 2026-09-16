namespace PortalOcorrenciasIPT.Models;

// Tabela de junção da relação muitos-para-muitos entre Ocorrencia e Impacto.
// A chave primária composta é configurada no ApplicationDbContext.
public class OcorrenciaImpacto
{
    // Chave estrangeira para a ocorrência.
    public int OcorrenciaId { get; set; }

    public Ocorrencia? Ocorrencia { get; set; }

    // Chave estrangeira para o impacto.
    public int ImpactoId { get; set; }

    public Impacto? Impacto { get; set; }
}