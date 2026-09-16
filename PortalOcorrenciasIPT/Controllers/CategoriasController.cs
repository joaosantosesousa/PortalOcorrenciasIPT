using Microsoft.AspNetCore.Mvc;
using PortalOcorrenciasIPT.Data;

namespace PortalOcorrenciasIPT.Controllers;

// Controller da API REST responsável pela consulta de categorias.
[ApiController]
[Route("api/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Endpoint público que devolve apenas categorias ativas.
    // É útil para formulários ou clientes externos que só devem usar categorias disponíveis.
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

    // Endpoint que devolve todas as categorias, incluindo inativas.
    // Permite consultar o estado completo das categorias existentes.
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

    // Endpoint para consultar uma categoria específica pelo seu Id.
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