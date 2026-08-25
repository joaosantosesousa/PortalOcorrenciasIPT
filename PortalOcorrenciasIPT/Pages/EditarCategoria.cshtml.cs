using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;

namespace PortalOcorrenciasIPT.Pages;

public class EditarCategoriaModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditarCategoriaModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Categoria Categoria { get; set; } = new();

    public IActionResult OnGet(int id)
    {
        Categoria? categoriaEncontrada = _context.Categorias
            .FirstOrDefault(categoria => categoria.Id == id);

        if (categoriaEncontrada == null)
        {
            return RedirectToPage("/Categorias");
        }

        Categoria = categoriaEncontrada;

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Categoria? categoriaExistente = _context.Categorias
            .FirstOrDefault(categoria => categoria.Id == Categoria.Id);

        if (categoriaExistente == null)
        {
            return RedirectToPage("/Categorias");
        }

        categoriaExistente.Nome = Categoria.Nome;
        categoriaExistente.Descricao = Categoria.Descricao;
        categoriaExistente.Ativa = Categoria.Ativa;

        _context.SaveChanges();

        TempData["MensagemSucesso"] = "Categoria atualizada com sucesso.";

        return RedirectToPage("/Categorias");
    }
}