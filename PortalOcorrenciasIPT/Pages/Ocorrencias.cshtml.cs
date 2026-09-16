using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;

namespace PortalOcorrenciasIPT.Pages;

// Página pública de listagem de ocorrências.
// Apresenta as ocorrências mais recentes e usa paginação server-side.
public class OcorrenciasModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public OcorrenciasModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Ocorrencia> Ocorrencias { get; set; } = new();

    // Dados usados pela interface para construir os botões de paginação.
    public int PaginaAtual { get; set; }

    public int TotalPaginas { get; set; }

    public int TotalOcorrencias { get; set; }

    // Número de ocorrências apresentadas por página.
    public const int TamanhoPagina = 5;

    public async Task OnGetAsync(int paginaAtual = 1)
    {
        // Garante que a página pedida nunca é inferior a 1.
        if (paginaAtual < 1)
        {
            paginaAtual = 1;
        }

        PaginaAtual = paginaAtual;

        // Query base da listagem.
        // Include carrega a categoria associada a cada ocorrência para ser apresentada na tabela/lista.
        var query = _context.Ocorrencias
            .Include(o => o.Categoria)
            .OrderByDescending(o => o.DataCriacao)
            .AsQueryable();

        // Conta o total de ocorrências antes da paginação,
        // permitindo calcular o número total de páginas.
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
    }
}