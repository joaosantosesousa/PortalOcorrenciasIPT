using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;

namespace PortalOcorrenciasIPT.Pages;

public class OcorrenciasModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public OcorrenciasModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Ocorrencia> Ocorrencias { get; set; } = new();

    public int PaginaAtual { get; set; }

    public int TotalPaginas { get; set; }

    public int TotalOcorrencias { get; set; }

    public const int TamanhoPagina = 5;

    public async Task OnGetAsync(int paginaAtual = 1)
    {
        if (paginaAtual < 1)
        {
            paginaAtual = 1;
        }

        PaginaAtual = paginaAtual;

        var query = _context.Ocorrencias
            .Include(o => o.Categoria)
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
    }
}