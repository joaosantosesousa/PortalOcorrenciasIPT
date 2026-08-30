using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;

namespace PortalOcorrenciasIPT.Pages;

public class EditarOcorrenciaModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditarOcorrenciaModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Ocorrencia Ocorrencia { get; set; } = new();

    public List<SelectListItem> CategoriasOpcoes { get; set; } = new();

    public List<SelectListItem> EstadosOpcoes { get; set; } = new();

    public List<SelectListItem> PrioridadesOpcoes { get; set; } = new();

    public IActionResult OnGet(int id)
    {
        Ocorrencia? ocorrenciaEncontrada = _context.Ocorrencias
            .FirstOrDefault(ocorrencia => ocorrencia.Id == id);

        if (ocorrenciaEncontrada == null)
        {
            return RedirectToPage("/Ocorrencias");
        }

        Ocorrencia = ocorrenciaEncontrada;

        CarregarOpcoes();

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            CarregarOpcoes();
            return Page();
        }

        Ocorrencia? ocorrenciaExistente = _context.Ocorrencias
            .FirstOrDefault(ocorrencia => ocorrencia.Id == Ocorrencia.Id);

        if (ocorrenciaExistente == null)
        {
            return RedirectToPage("/Ocorrencias");
        }

        ocorrenciaExistente.Titulo = Ocorrencia.Titulo;
        ocorrenciaExistente.Descricao = Ocorrencia.Descricao;
        ocorrenciaExistente.LocalizacaoTexto = Ocorrencia.LocalizacaoTexto;
        ocorrenciaExistente.Edificio = Ocorrencia.Edificio;
        ocorrenciaExistente.CategoriaId = Ocorrencia.CategoriaId;
        ocorrenciaExistente.Estado = Ocorrencia.Estado;
        ocorrenciaExistente.Prioridade = Ocorrencia.Prioridade;

        _context.SaveChanges();

        TempData["MensagemSucesso"] = "Ocorrência atualizada com sucesso.";

        return RedirectToPage("/Ocorrencias");
    }

    private void CarregarOpcoes()
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

        EstadosOpcoes = new List<SelectListItem>
        {
            new SelectListItem { Value = "Aberta", Text = "Aberta" },
            new SelectListItem { Value = "Em análise", Text = "Em análise" },
            new SelectListItem { Value = "Em resolução", Text = "Em resolução" },
            new SelectListItem { Value = "Resolvida", Text = "Resolvida" },
            new SelectListItem { Value = "Encerrada", Text = "Encerrada" }
        };

        PrioridadesOpcoes = new List<SelectListItem>
        {
            new SelectListItem { Value = "Baixa", Text = "Baixa" },
            new SelectListItem { Value = "Normal", Text = "Normal" },
            new SelectListItem { Value = "Alta", Text = "Alta" },
            new SelectListItem { Value = "Urgente", Text = "Urgente" }
        };
    }
}