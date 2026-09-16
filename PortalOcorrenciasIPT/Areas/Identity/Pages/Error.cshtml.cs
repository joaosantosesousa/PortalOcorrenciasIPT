// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PortalOcorrenciasIPT.Areas.Identity.Pages;

// Página de erro associada à área Identity.
// Mantém acesso anónimo para permitir apresentar erros mesmo sem autenticação.
[AllowAnonymous]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public void OnGet()
    {
        // Guarda o identificador do pedido para ajudar no diagnóstico de erros.
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}