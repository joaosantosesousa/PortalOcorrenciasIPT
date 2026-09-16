using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using PortalOcorrenciasIPT.Hubs;

namespace PortalOcorrenciasIPT.Pages;

// Apenas utilizadores autenticados podem criar ocorrências.
[Authorize]
public class CriarOcorrenciaModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHubContext<OcorrenciasHub> _hubContext;

    public CriarOcorrenciaModel(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IHubContext<OcorrenciasHub> hubContext)
    {
        _context = context;
        _userManager = userManager;
        _hubContext = hubContext;
    }

    // Objeto preenchido pelo formulário de criação da ocorrência.
    [BindProperty]
    public Ocorrencia Ocorrencia { get; set; } = new();

    // Lista auxiliar usada para receber os impactos selecionados nas checkboxes.
    [BindProperty]
    public List<ImpactoCheckbox> ImpactosCheckbox { get; set; } = new();

    // Lista de categorias ativas apresentada no dropdown do formulário.
    public List<SelectListItem> CategoriasOpcoes { get; set; } = new();

    public void OnGet()
    {
        CarregarCategorias();
        CarregarImpactos();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Se existirem erros de validação, os dados auxiliares do formulário
        // têm de ser carregados novamente antes de devolver a página.
        if (!ModelState.IsValid)
        {
            CarregarCategorias();

            if (ImpactosCheckbox == null || ImpactosCheckbox.Count == 0)
            {
                CarregarImpactos();
            }

            return Page();
        }

        // Obtém o utilizador autenticado para associar a ocorrência ao seu autor.
        ApplicationUser? utilizadorAtual = await _userManager.GetUserAsync(User);

        if (utilizadorAtual == null)
        {
            return RedirectToPage("/Index");
        }

        // Alguns campos são definidos pela aplicação e não pelo utilizador.
        Ocorrencia.DataCriacao = DateTime.Now;
        Ocorrencia.Estado = "Aberta";
        Ocorrencia.UtilizadorId = utilizadorAtual.Id;

        // Primeiro guarda-se a ocorrência para obter o seu Id.
        _context.Ocorrencias.Add(Ocorrencia);
        _context.SaveChanges();

        // Depois são criados os registos da tabela de junção OcorrenciaImpacto,
        // representando a relação muitos-para-muitos entre ocorrências e impactos.
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

        // Notifica todos os clientes ligados ao hub SignalR
        // de que foi criada uma nova ocorrência.
        await _hubContext.Clients.All.SendAsync(
            "NovaOcorrencia",
            Ocorrencia.Titulo,
            Ocorrencia.LocalizacaoTexto);

        TempData["MensagemSucesso"] = "Ocorrência criada com sucesso.";

        return RedirectToPage("/Ocorrencias");
    }

    // Carrega apenas categorias ativas para impedir a criação de ocorrências
    // associadas a categorias desativadas.
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

    // Carrega os impactos ativos como ViewModels auxiliares,
    // permitindo apresentá-los no formulário sob a forma de checkboxes.
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