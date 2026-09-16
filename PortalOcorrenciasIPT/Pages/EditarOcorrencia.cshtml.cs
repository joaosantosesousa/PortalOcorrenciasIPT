using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;
using Microsoft.AspNetCore.Authorization;

namespace PortalOcorrenciasIPT.Pages;

// Apenas utilizadores com a role Gestor podem editar ocorrências.
// Esta página representa a parte de gestão da ocorrência.
[Authorize(Roles = "Gestor")]
public class EditarOcorrenciaModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditarOcorrenciaModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Ocorrência preenchida pelo formulário de edição.
    [BindProperty]
    public Ocorrencia Ocorrencia { get; set; } = new();

    // Lista auxiliar usada para apresentar e receber os impactos selecionados.
    [BindProperty]
    public List<ImpactoCheckbox> ImpactosCheckbox { get; set; } = new();

    public List<SelectListItem> CategoriasOpcoes { get; set; } = new();

    public List<SelectListItem> EstadosOpcoes { get; set; } = new();

    public List<SelectListItem> PrioridadesOpcoes { get; set; } = new();

    public IActionResult OnGet(int id)
    {
        // Carrega a ocorrência a editar juntamente com os impactos já associados.
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
        // Em caso de erro de validação, as listas auxiliares têm de ser recarregadas
        // para que o formulário volte a apresentar dropdowns e checkboxes corretamente.
        if (!ModelState.IsValid)
        {
            CarregarOpcoes();

            if (ImpactosCheckbox == null || ImpactosCheckbox.Count == 0)
            {
                CarregarImpactos(Ocorrencia.Id);
            }

            return Page();
        }

        // A ocorrência é novamente carregada a partir da base de dados para garantir
        // que estamos a atualizar um registo existente e controlado pelo EF Core.
        Ocorrencia? ocorrenciaExistente = _context.Ocorrencias
            .Include(ocorrencia => ocorrencia.OcorrenciaImpactos)
            .FirstOrDefault(ocorrencia => ocorrencia.Id == Ocorrencia.Id);

        if (ocorrenciaExistente == null)
        {
            return RedirectToPage("/Ocorrencias");
        }

        // Atualização dos campos editáveis pelo Gestor.
        ocorrenciaExistente.Titulo = Ocorrencia.Titulo;
        ocorrenciaExistente.Descricao = Ocorrencia.Descricao;
        ocorrenciaExistente.LocalizacaoTexto = Ocorrencia.LocalizacaoTexto;
        ocorrenciaExistente.Edificio = Ocorrencia.Edificio;
        ocorrenciaExistente.CategoriaId = Ocorrencia.CategoriaId;
        ocorrenciaExistente.Estado = Ocorrencia.Estado;
        ocorrenciaExistente.Prioridade = Ocorrencia.Prioridade;

        // Atualização da relação muitos-para-muitos.
        // Primeiro removem-se as associações antigas e depois são inseridas
        // as associações atualmente selecionadas no formulário.
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

    // Carrega as listas usadas nos dropdowns do formulário.
    // As categorias apresentadas são apenas as categorias ativas.
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

        // Estados possíveis do ciclo de vida de uma ocorrência.
        EstadosOpcoes = new List<SelectListItem>
        {
            new SelectListItem { Value = "Aberta", Text = "Aberta" },
            new SelectListItem { Value = "Em análise", Text = "Em análise" },
            new SelectListItem { Value = "Em resolução", Text = "Em resolução" },
            new SelectListItem { Value = "Resolvida", Text = "Resolvida" },
            new SelectListItem { Value = "Encerrada", Text = "Encerrada" }
        };

        // Níveis de prioridade usados pelo Gestor para classificar a ocorrência.
        PrioridadesOpcoes = new List<SelectListItem>
        {
            new SelectListItem { Value = "Baixa", Text = "Baixa" },
            new SelectListItem { Value = "Normal", Text = "Normal" },
            new SelectListItem { Value = "Alta", Text = "Alta" },
            new SelectListItem { Value = "Urgente", Text = "Urgente" }
        };
    }

    // Carrega os impactos disponíveis e marca como selecionados
    // os que já estão associados à ocorrência.
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