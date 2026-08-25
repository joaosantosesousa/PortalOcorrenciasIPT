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

    public List<SelectListItem> CategoriasOpcoes { get; set; } = new();

    public void OnGet()
    {
        CarregarCategorias();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            CarregarCategorias();
            return Page();
        }

        Ocorrencia.DataCriacao = DateTime.Now;
        Ocorrencia.Estado = "Aberta";

        _context.Ocorrencias.Add(Ocorrencia);
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
}