using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;
using Microsoft.AspNetCore.Authorization;

namespace PortalOcorrenciasIPT.Pages;

[Authorize(Roles = "Gestor")]
public class CategoriasModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CategoriasModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Categoria> Categorias { get; set; } = new();

    public void OnGet()
    {
        Categorias = _context.Categorias.ToList();
    }
}