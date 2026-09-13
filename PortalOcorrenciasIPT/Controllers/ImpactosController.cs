using Microsoft.AspNetCore.Mvc;
using PortalOcorrenciasIPT.Data;

namespace PortalOcorrenciasIPT.Controllers;

[ApiController]
[Route("api/impactos")]
public class ImpactosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ImpactosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetImpactosAtivos()
    {
        var impactos = _context.Impactos
            .Where(impacto => impacto.Ativo)
            .OrderBy(impacto => impacto.Nome)
            .Select(impacto => new
            {
                impacto.Id,
                impacto.Nome,
                impacto.Descricao
            })
            .ToList();

        return Ok(impactos);
    }

    [HttpGet("todos")]
    public IActionResult GetTodosImpactos()
    {
        var impactos = _context.Impactos
            .OrderBy(impacto => impacto.Nome)
            .Select(impacto => new
            {
                impacto.Id,
                impacto.Nome,
                impacto.Descricao,
                impacto.Ativo
            })
            .ToList();

        return Ok(impactos);
    }

    [HttpGet("{id}")]
    public IActionResult GetImpacto(int id)
    {
        var impacto = _context.Impactos
            .Where(impacto => impacto.Id == id)
            .Select(impacto => new
            {
                impacto.Id,
                impacto.Nome,
                impacto.Descricao,
                impacto.Ativo
            })
            .FirstOrDefault();

        if (impacto == null)
        {
            return NotFound();
        }

        return Ok(impacto);
    }
}