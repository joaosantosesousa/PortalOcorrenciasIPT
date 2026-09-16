namespace PortalOcorrenciasIPT.Models;

// ViewModel usado nos formulários de criação/edição de ocorrências.
// Permite apresentar cada impacto como uma checkbox e guardar se foi selecionado.
public class ImpactoCheckbox
{
    public int Id { get; set; }

    public string Nome { get; set; } = "";

    public bool Selecionado { get; set; }
}