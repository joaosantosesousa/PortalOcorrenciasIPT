using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.Models;
using Microsoft.AspNetCore.Authorization;

namespace PortalOcorrenciasIPT.Pages;

// Apenas utilizadores com a role Gestor podem editar categorias.
[Authorize(Roles = "Gestor")]
public class EditarCategoriaModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditarCategoriaModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Categoria preenchida pelo formulário de edição.
    [BindProperty]
    public Categoria Categoria { get; set; } = new();

    public IActionResult OnGet(int id)
    {
        // Carrega a categoria existente para preencher o formulário.
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
        // Valida os dados submetidos antes de atualizar a base de dados.
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // A categoria é novamente obtida da base de dados para garantir
        // que estamos a alterar um registo existente.
        Categoria? categoriaExistente = _context.Categorias
            .FirstOrDefault(categoria => categoria.Id == Categoria.Id);

        if (categoriaExistente == null)
        {
            return RedirectToPage("/Categorias");
        }

        // Atualização dos campos editáveis da categoria.
        // O campo Ativa permite desativar categorias sem as remover da base de dados.
        categoriaExistente.Nome = Categoria.Nome;
        categoriaExistente.Descricao = Categoria.Descricao;
        categoriaExistente.Ativa = Categoria.Ativa;

        _context.SaveChanges();

        TempData["MensagemSucesso"] = "Categoria atualizada com sucesso.";

        return RedirectToPage("/Categorias");
    }
}