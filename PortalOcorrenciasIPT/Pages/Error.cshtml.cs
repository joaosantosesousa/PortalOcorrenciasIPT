using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PortalOcorrenciasIPT.Pages;

// Página de erro geral da aplicação.
// É usada para apresentar erros de forma controlada em produção,
// incluindo códigos HTTP como 404 ou 403.
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }

    // Código HTTP recebido quando a página é chamada através de StatusCodePagesWithReExecute.
    public int? StatusCode { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public void OnGet(int? statusCode = null)
    {
        StatusCode = statusCode;

        // Guarda o identificador do pedido para facilitar diagnóstico em caso de erro.
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}