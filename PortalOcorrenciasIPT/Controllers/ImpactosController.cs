using Microsoft.AspNetCore.Mvc;
using PortalOcorrenciasIPT.Data;

namespace PortalOcorrenciasIPT.Controllers;

// Controller da API REST responsável pela consulta dos impactos.
// Os impactos são usados na relação muitos-para-muitos com as ocorrências.
[ApiController]
[Route("api/impactos")]
public class ImpactosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ImpactosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Endpoint público que devolve apenas impactos ativos.
    // Estes são os impactos disponíveis para associar a novas ocorrências.
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

    // Endpoint que devolve todos os impactos, incluindo inativos.
    // É útil para consulta completa dos dados existentes.
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

    // Endpoint para consultar um impacto específico pelo seu Id.
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