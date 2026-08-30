using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;

namespace PortalOcorrenciasIPT.Pages;

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
        Categoria? categoria = _context.Categorias
            .FirstOrDefault(categoria => categoria.Id == id);

        if (categoria == null)
        {
            return RedirectToPage("/Categorias");
        }

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