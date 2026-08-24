using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;

namespace PortalOcorrenciasIPT.Pages;

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