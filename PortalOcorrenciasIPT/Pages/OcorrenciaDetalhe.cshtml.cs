using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;

namespace PortalOcorrenciasIPT.Pages;

public class OcorrenciaDetalheModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public OcorrenciaDetalheModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Ocorrencia? Ocorrencia { get; set; }

    public IActionResult OnGet(int id)
    {
        Ocorrencia = _context.Ocorrencias
            .Include(ocorrencia => ocorrencia.Categoria)
            .Include(ocorrencia => ocorrencia.Utilizador)
            .Include(ocorrencia => ocorrencia.OcorrenciaImpactos)
            .ThenInclude(ocorrenciaImpacto => ocorrenciaImpacto.Impacto)
            .FirstOrDefault(ocorrencia => ocorrencia.Id == id);

        if (Ocorrencia == null)
        {
            return RedirectToPage("/Ocorrencias");
        }

        return Page();
    }

    public IActionResult OnPostApoiar(int id)
    {
        Ocorrencia? ocorrencia = _context.Ocorrencias
            .FirstOrDefault(ocorrencia => ocorrencia.Id == id);

        if (ocorrencia == null)
        {
            return RedirectToPage("/Ocorrencias");
        }

        ocorrencia.NumeroApoios++;

        _context.SaveChanges();

        TempData["MensagemSucesso"] = "Obrigado. O seu apoio foi registado.";

        return RedirectToPage("/OcorrenciaDetalhe", new { id = id });
    }
}