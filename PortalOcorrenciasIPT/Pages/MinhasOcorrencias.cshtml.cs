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

    public async Task<IActionResult> OnGetAsync()
    {
        ApplicationUser? utilizadorAtual = await _userManager.GetUserAsync(User);

        if (utilizadorAtual == null)
        {
            return RedirectToPage("/Index");
        }

        Ocorrencias = _context.Ocorrencias
            .Include(ocorrencia => ocorrencia.Categoria)
            .Where(ocorrencia => ocorrencia.UtilizadorId == utilizadorAtual.Id)
            .OrderByDescending(ocorrencia => ocorrencia.DataCriacao)
            .ToList();

        return Page();
    }
}