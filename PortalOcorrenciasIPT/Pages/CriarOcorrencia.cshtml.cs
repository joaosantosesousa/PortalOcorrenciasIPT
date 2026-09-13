using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


namespace PortalOcorrenciasIPT.Pages;

[Authorize]
public class CriarOcorrenciaModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CriarOcorrenciaModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public Ocorrencia Ocorrencia { get; set; } = new();

    [BindProperty]
    public List<ImpactoCheckbox> ImpactosCheckbox { get; set; } = new();

    public List<SelectListItem> CategoriasOpcoes { get; set; } = new();

    public void OnGet()
    {
        CarregarCategorias();
        CarregarImpactos();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            CarregarCategorias();

            if (ImpactosCheckbox == null || ImpactosCheckbox.Count == 0)
            {
                CarregarImpactos();
            }

            return Page();
        }

        ApplicationUser? utilizadorAtual = await _userManager.GetUserAsync(User);

        if (utilizadorAtual == null)
        {
            return RedirectToPage("/Index");
        }

        Ocorrencia.DataCriacao = DateTime.Now;
        Ocorrencia.Estado = "Aberta";
        Ocorrencia.UtilizadorId = utilizadorAtual.Id;

        _context.Ocorrencias.Add(Ocorrencia);
        _context.SaveChanges();

        foreach (var impactoCheckbox in ImpactosCheckbox.Where(impacto => impacto.Selecionado))
        {
            var ocorrenciaImpacto = new OcorrenciaImpacto
            {
                OcorrenciaId = Ocorrencia.Id,
                ImpactoId = impactoCheckbox.Id
            };

            _context.OcorrenciaImpactos.Add(ocorrenciaImpacto);
        }

        _context.SaveChanges();

        TempData["MensagemSucesso"] = "Ocorrência criada com sucesso.";

        return RedirectToPage("/Ocorrencias");
    }

    private void CarregarCategorias()
    {
        CategoriasOpcoes = _context.Categorias
            .Where(categoria => categoria.Ativa)
            .OrderBy(categoria => categoria.Nome)
            .Select(categoria => new SelectListItem
            {
                Value = categoria.Id.ToString(),
                Text = categoria.Nome
            })
            .ToList();
    }

    private void CarregarImpactos()
    {
        ImpactosCheckbox = _context.Impactos
            .Where(impacto => impacto.Ativo)
            .OrderBy(impacto => impacto.Nome)
            .Select(impacto => new ImpactoCheckbox
            {
                Id = impacto.Id,
                Nome = impacto.Nome,
                Selecionado = false
            })
            .ToList();
    }
}