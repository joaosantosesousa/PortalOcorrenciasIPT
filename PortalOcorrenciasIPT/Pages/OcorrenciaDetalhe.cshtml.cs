using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;

namespace PortalOcorrenciasIPT.Pages;

// Página pública de detalhe de uma ocorrência.
// Permite consultar informação completa, apoiar a ocorrência e adicionar comentários.
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

    // Comentário preenchido pelo formulário da página.
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
        // A opção "Também sou afetado" é pública e incrementa apenas um contador.
        // Foi mantida simples para permitir participação sem autenticação.
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
        // Os comentários exigem autenticação para manter autoria e rastreabilidade.
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Challenge();
        }

        ApplicationUser? utilizadorAtual = await _userManager.GetUserAsync(User);

        if (utilizadorAtual == null)
        {
            return Challenge();
        }

        // Se o comentário não for válido, a ocorrência tem de ser carregada novamente
        // para a página voltar a apresentar todos os dados.
        if (!ModelState.IsValid)
        {
            CarregarOcorrencia(id);

            if (Ocorrencia == null)
            {
                return RedirectToPage("/Ocorrencias");
            }

            return Page();
        }

        // Criação do comentário associado à ocorrência e ao utilizador autenticado.
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
        // Carrega a ocorrência com os dados relacionados necessários para a página de detalhe:
        // categoria, autor, impactos associados e comentários com respetivos autores.
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