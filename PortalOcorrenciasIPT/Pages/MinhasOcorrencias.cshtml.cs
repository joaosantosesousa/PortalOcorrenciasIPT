using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;

namespace PortalOcorrenciasIPT.Pages;

[Authorize]
public class MinhasOcorrenciasModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MinhasOcorrenciasModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<Ocorrencia> Ocorrencias { get; set; } = new();

    public int PaginaAtual { get; set; }

    public int TotalPaginas { get; set; }

    public int TotalOcorrencias { get; set; }

    public const int TamanhoPagina = 5;

    public async Task<IActionResult> OnGetAsync(int paginaAtual = 1)
    {
        var utilizador = await _userManager.GetUserAsync(User);

        if (utilizador == null)
        {
            return RedirectToPage("/Identity/Account/Login");
        }

        if (paginaAtual < 1)
        {
            paginaAtual = 1;
        }

        PaginaAtual = paginaAtual;

        var query = _context.Ocorrencias
            .Include(o => o.Categoria)
            .Where(o => o.UtilizadorId == utilizador.Id)
            .OrderByDescending(o => o.DataCriacao)
            .AsQueryable();

        TotalOcorrencias = await query.CountAsync();

        TotalPaginas = (int)Math.Ceiling(TotalOcorrencias / (double)TamanhoPagina);

        if (TotalPaginas > 0 && PaginaAtual > TotalPaginas)
        {
            PaginaAtual = TotalPaginas;
        }

        Ocorrencias = await query
            .Skip((PaginaAtual - 1) * TamanhoPagina)
            .Take(TamanhoPagina)
            .ToListAsync();

        return Page();
    }
}