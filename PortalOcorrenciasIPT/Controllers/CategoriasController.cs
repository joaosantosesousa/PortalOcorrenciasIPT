using Microsoft.AspNetCore.Mvc;
using PortalOcorrenciasIPT.Data;

namespace PortalOcorrenciasIPT.Controllers;

[ApiController]
[Route("api/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriasController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetCategoriasAtivas()
    {
        var categorias = _context.Categorias
            .Where(categoria => categoria.Ativa)
            .OrderBy(categoria => categoria.Nome)
            .Select(categoria => new
            {
                categoria.Id,
                categoria.Nome,
                categoria.Descricao
            })
            .ToList();

        return Ok(categorias);
    }

    [HttpGet("todas")]
    public IActionResult GetTodasCategorias()
    {
        var categorias = _context.Categorias
            .OrderBy(categoria => categoria.Nome)
            .Select(categoria => new
            {
                categoria.Id,
                categoria.Nome,
                categoria.Descricao,
                categoria.Ativa
            })
            .ToList();

        return Ok(categorias);
    }

    [HttpGet("{id}")]
    public IActionResult GetCategoria(int id)
    {
        var categoria = _context.Categorias
            .Where(categoria => categoria.Id == id)
            .Select(categoria => new
            {
                categoria.Id,
                categoria.Nome,
                categoria.Descricao,
                categoria.Ativa
            })
            .FirstOrDefault();

        if (categoria == null)
        {
            return NotFound();
        }

        return Ok(categoria);
    }
}