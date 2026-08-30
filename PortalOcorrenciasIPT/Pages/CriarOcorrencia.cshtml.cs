using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;

namespace PortalOcorrenciasIPT.Pages;

public class CriarOcorrenciaModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CriarOcorrenciaModel(ApplicationDbContext context)
    {
        _context = context;
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

    public IActionResult OnPost()
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

        Ocorrencia.DataCriacao = DateTime.Now;
        Ocorrencia.Estado = "Aberta";

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