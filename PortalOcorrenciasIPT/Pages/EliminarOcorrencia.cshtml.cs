using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;
using Microsoft.AspNetCore.Authorization;

namespace PortalOcorrenciasIPT.Pages;

// Apenas utilizadores com a role Gestor podem eliminar ocorrências.
[Authorize(Roles = "Gestor")]
public class EliminarOcorrenciaModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EliminarOcorrenciaModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Ocorrencia? Ocorrencia { get; set; }

    public IActionResult OnGet(int id)
    {
        // Carrega a ocorrência, incluindo a categoria,
        // para apresentar os dados na página de confirmação de eliminação.
        Ocorrencia = _context.Ocorrencias
            .Include(ocorrencia => ocorrencia.Categoria)
            .FirstOrDefault(ocorrencia => ocorrencia.Id == id);

        if (Ocorrencia == null)
        {
            return RedirectToPage("/Ocorrencias");
        }

        return Page();
    }

    public IActionResult OnPost(int id)
    {
        // Volta a obter a ocorrência no momento da submissão,
        // garantindo que o registo ainda existe antes de tentar removê-lo.
        Ocorrencia? ocorrencia = _context.Ocorrencias
            .FirstOrDefault(ocorrencia => ocorrencia.Id == id);

        if (ocorrencia == null)
        {
            return RedirectToPage("/Ocorrencias");
        }

        // Remove a ocorrência da base de dados.
        // Os comentários associados são eliminados por cascata,
        // conforme configurado no ApplicationDbContext.
        _context.Ocorrencias.Remove(ocorrencia);
        _context.SaveChanges();

        TempData["MensagemSucesso"] = "Ocorrência eliminada com sucesso.";

        return RedirectToPage("/Ocorrencias");
    }
}