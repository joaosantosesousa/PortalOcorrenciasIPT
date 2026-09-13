using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;

namespace PortalOcorrenciasIPT.Pages;

public class OcorrenciaDetalheModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OcorrenciaDetalheModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Ocorrencia? Ocorrencia { get; set; }

    [BindProperty]
    public Comentario NovoComentario { get; set; } = new();

    public IActionResult OnGet(int id)
    {
        CarregarOcorrencia(id);

        if (Ocorrencia == null)
        {
            return RedirectToPage("/Ocorrencias");
        }

        return Page();
    }

    public IActionResult OnPostApoiar(int id)
    {
        Ocorrencia? ocorrencia = _context.Ocorrencias
            .FirstOrDefault(ocorrencia => ocorrencia.Id == id);

        if (ocorrencia == null)
        {
            return RedirectToPage("/Ocorrencias");
        }

        ocorrencia.NumeroApoios++;

        _context.SaveChanges();

        TempData["MensagemSucesso"] = "Obrigado. O seu apoio foi registado.";

        return RedirectToPage("/OcorrenciaDetalhe", new { id = id });
    }

    public async Task<IActionResult> OnPostComentarAsync(int id)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Challenge();
        }

        ApplicationUser? utilizadorAtual = await _userManager.GetUserAsync(User);

        if (utilizadorAtual == null)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            CarregarOcorrencia(id);

            if (Ocorrencia == null)
            {
                return RedirectToPage("/Ocorrencias");
            }

            return Page();
        }

        Comentario comentario = new Comentario
        {
            Texto = NovoComentario.Texto,
            DataCriacao = DateTime.Now,
            OcorrenciaId = id,
            UtilizadorId = utilizadorAtual.Id
        };

        _context.Comentarios.Add(comentario);
        _context.SaveChanges();

        TempData["MensagemSucesso"] = "Comentário adicionado com sucesso.";

        return RedirectToPage("/OcorrenciaDetalhe", new { id = id });
    }

    private void CarregarOcorrencia(int id)
    {
        Ocorrencia = _context.Ocorrencias
            .Include(ocorrencia => ocorrencia.Categoria)
            .Include(ocorrencia => ocorrencia.Utilizador)
            .Include(ocorrencia => ocorrencia.OcorrenciaImpactos)
                .ThenInclude(ocorrenciaImpacto => ocorrenciaImpacto.Impacto)
            .Include(ocorrencia => ocorrencia.Comentarios)
                .ThenInclude(comentario => comentario.Utilizador)
            .FirstOrDefault(ocorrencia => ocorrencia.Id == id);
    }
}