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

    public void OnGet()
    {
        Ocorrencias = _context.Ocorrencias
            .Include(ocorrencia => ocorrencia.Categoria)
            .OrderByDescending(ocorrencia => ocorrencia.DataCriacao)
            .ToList();
    }
}
