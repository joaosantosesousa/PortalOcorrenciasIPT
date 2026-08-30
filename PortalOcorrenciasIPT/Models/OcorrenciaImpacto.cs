namespace PortalOcorrenciasIPT.Models;

public class OcorrenciaImpacto
{
    public int OcorrenciaId { get; set; }

    public Ocorrencia? Ocorrencia { get; set; }

    public int ImpactoId { get; set; }

    public Impacto? Impacto { get; set; }
}