using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;
using Microsoft.AspNetCore.Authorization;

namespace PortalOcorrenciasIPT.Pages;

[Authorize(Roles = "Gestor")]

public class EditarOcorrenciaModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditarOcorrenciaModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Ocorrencia Ocorrencia { get; set; } = new();

    [BindProperty]
    public List<ImpactoCheckbox> ImpactosCheckbox { get; set; } = new();

    public List<SelectListItem> CategoriasOpcoes { get; set; } = new();

    public List<SelectListItem> EstadosOpcoes { get; set; } = new();

    public List<SelectListItem> PrioridadesOpcoes { get; set; } = new();

    public IActionResult OnGet(int id)
    {
        Ocorrencia? ocorrenciaEncontrada = _context.Ocorrencias
            .Include(ocorrencia => ocorrencia.OcorrenciaImpactos)
            .FirstOrDefault(ocorrencia => ocorrencia.Id == id);

        if (ocorrenciaEncontrada == null)
        {
            return RedirectToPage("/Ocorrencias");
        }

        Ocorrencia = ocorrenciaEncontrada;

        CarregarOpcoes();
        CarregarImpactos(id);

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            CarregarOpcoes();

            if (ImpactosCheckbox == null || ImpactosCheckbox.Count == 0)
            {
                CarregarImpactos(Ocorrencia.Id);
            }

            return Page();
        }

        Ocorrencia? ocorrenciaExistente = _context.Ocorrencias
            .Include(ocorrencia => ocorrencia.OcorrenciaImpactos)
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

        _context.OcorrenciaImpactos.RemoveRange(ocorrenciaExistente.OcorrenciaImpactos);

        foreach (var impactoCheckbox in ImpactosCheckbox.Where(impacto => impacto.Selecionado))
        {
            var ocorrenciaImpacto = new OcorrenciaImpacto
            {
                OcorrenciaId = ocorrenciaExistente.Id,
                ImpactoId = impactoCheckbox.Id
            };

            _context.OcorrenciaImpactos.Add(ocorrenciaImpacto);
        }

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

    private void CarregarImpactos(int ocorrenciaId)
    {
        List<int> impactosSelecionados = _context.OcorrenciaImpactos
            .Where(ocorrenciaImpacto => ocorrenciaImpacto.OcorrenciaId == ocorrenciaId)
            .Select(ocorrenciaImpacto => ocorrenciaImpacto.ImpactoId)
            .ToList();

        ImpactosCheckbox = _context.Impactos
            .Where(impacto => impacto.Ativo)
            .OrderBy(impacto => impacto.Nome)
            .Select(impacto => new ImpactoCheckbox
            {
                Id = impacto.Id,
                Nome = impacto.Nome,
                Selecionado = impactosSelecionados.Contains(impacto.Id)
            })
            .ToList();
    }
}