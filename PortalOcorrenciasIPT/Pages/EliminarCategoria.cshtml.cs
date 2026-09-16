using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;
using Microsoft.AspNetCore.Authorization;

namespace PortalOcorrenciasIPT.Pages;

// Apenas utilizadores com a role Gestor podem eliminar categorias.
[Authorize(Roles = "Gestor")]
public class EliminarCategoriaModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EliminarCategoriaModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Categoria? Categoria { get; set; }

    public IActionResult OnGet(int id)
    {
        // Carrega a categoria para apresentar a página de confirmação de eliminação.
        Categoria = _context.Categorias
            .FirstOrDefault(categoria => categoria.Id == id);

        if (Categoria == null)
        {
            return RedirectToPage("/Categorias");
        }

        return Page();
    }

    public IActionResult OnPost(int id)
    {
        // Volta a obter a categoria no momento da submissão,
        // garantindo que o registo ainda existe antes de tentar removê-lo.
        Categoria? categoria = _context.Categorias
            .FirstOrDefault(categoria => categoria.Id == id);

        if (categoria == null)
        {
            return RedirectToPage("/Categorias");
        }

        // Regra de integridade: uma categoria com ocorrências associadas não deve ser apagada,
        // porque isso deixaria ocorrências sem a respetiva classificação.
        bool temOcorrencias = _context.Ocorrencias
            .Any(ocorrencia => ocorrencia.CategoriaId == id);

        if (temOcorrencias)
        {
            TempData["MensagemErro"] = "Não é possível eliminar esta categoria porque existem ocorrências associadas. Pode marcá-la como inativa.";

            return RedirectToPage("/Categorias");
        }

        _context.Categorias.Remove(categoria);
        _context.SaveChanges();

        TempData["MensagemSucesso"] = "Categoria eliminada com sucesso.";

        return RedirectToPage("/Categorias");
    }
}