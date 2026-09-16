using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;

namespace PortalOcorrenciasIPT.Pages;

// Página acessível apenas a utilizadores autenticados.
// Lista apenas as ocorrências criadas pelo utilizador atual.
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

    // Dados usados pela interface para apresentar a paginação.
    public int PaginaAtual { get; set; }

    public int TotalPaginas { get; set; }

    public int TotalOcorrencias { get; set; }

    // Número de ocorrências apresentadas por página.
    public const int TamanhoPagina = 5;

    public async Task<IActionResult> OnGetAsync(int paginaAtual = 1)
    {
        // Obtém o utilizador autenticado para filtrar as ocorrências pelo seu Id.
        var utilizador = await _userManager.GetUserAsync(User);

        if (utilizador == null)
        {
            return RedirectToPage("/Identity/Account/Login");
        }

        // Garante que a página pedida nunca é inferior a 1.
        if (paginaAtual < 1)
        {
            paginaAtual = 1;
        }

        PaginaAtual = paginaAtual;

        // Query base da página "Minhas Ocorrências".
        // Primeiro filtra pelo utilizador autenticado e só depois aplica a paginação.
        var query = _context.Ocorrencias
            .Include(o => o.Categoria)
            .Where(o => o.UtilizadorId == utilizador.Id)
            .OrderByDescending(o => o.DataCriacao)
            .AsQueryable();

        // Conta apenas as ocorrências do utilizador atual.
        TotalOcorrencias = await query.CountAsync();

        TotalPaginas = (int)Math.Ceiling(TotalOcorrencias / (double)TamanhoPagina);

        // Se for pedido um número de página superior ao total existente,
        // a aplicação ajusta para a última página disponível.
        if (TotalPaginas > 0 && PaginaAtual > TotalPaginas)
        {
            PaginaAtual = TotalPaginas;
        }

        // Paginação server-side:
        // Skip ignora os registos das páginas anteriores e Take carrega apenas os da página atual.
        Ocorrencias = await query
            .Skip((PaginaAtual - 1) * TamanhoPagina)
            .Take(TamanhoPagina)
            .ToListAsync();

        return Page();
    }
}