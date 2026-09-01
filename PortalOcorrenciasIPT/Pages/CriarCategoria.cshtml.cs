using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;
using Microsoft.AspNetCore.Authorization;

namespace PortalOcorrenciasIPT.Pages;

[Authorize(Roles = "Gestor")]
public class CriarCategoriaModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CriarCategoriaModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Categoria Categoria { get; set; } = new();

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Categorias.Add(Categoria);
        _context.SaveChanges();

        TempData["MensagemSucesso"] = "Categoria criada com sucesso.";

        return RedirectToPage("/Categorias");
    }
}