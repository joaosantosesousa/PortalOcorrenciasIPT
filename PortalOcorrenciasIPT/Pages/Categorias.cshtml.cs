using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;
using Microsoft.AspNetCore.Authorization;

namespace PortalOcorrenciasIPT.Pages;

// Página de gestão de categorias.
// Apenas utilizadores com a role Gestor podem consultar e administrar categorias.
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
        // Carrega todas as categorias, incluindo ativas e inativas,
        // para permitir ao Gestor consultar o estado de cada uma.
        Categorias = _context.Categorias
    .OrderBy(categoria => categoria.Nome)
    .ToList();
    }
}