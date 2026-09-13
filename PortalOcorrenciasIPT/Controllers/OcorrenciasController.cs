using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalOcorrenciasIPT.Data;

namespace PortalOcorrenciasIPT.Controllers;

[ApiController]
[Route("api/ocorrencias")]
public class OcorrenciasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OcorrenciasController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetOcorrencias()
    {
        var ocorrencias = _context.Ocorrencias
            .Include(ocorrencia => ocorrencia.Categoria)
            .Include(ocorrencia => ocorrencia.OcorrenciaImpactos)
                .ThenInclude(ocorrenciaImpacto => ocorrenciaImpacto.Impacto)
            .OrderByDescending(ocorrencia => ocorrencia.DataCriacao)
            .Select(ocorrencia => new
            {
                ocorrencia.Id,
                ocorrencia.Titulo,
                ocorrencia.LocalizacaoTexto,
                ocorrencia.Estado,
                ocorrencia.Prioridade,
                ocorrencia.NumeroApoios,
                DataCriacao = ocorrencia.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                Categoria = ocorrencia.Categoria != null
                    ? ocorrencia.Categoria.Nome
                    : null,
                Impactos = ocorrencia.OcorrenciaImpactos
                    .Select(ocorrenciaImpacto => ocorrenciaImpacto.Impacto != null
                        ? ocorrenciaImpacto.Impacto.Nome
                        : "")
                    .Where(nome => nome != "")
                    .ToList()
            })
            .ToList();

        return Ok(ocorrencias);
    }

    [HttpGet("{id}")]
    public IActionResult GetOcorrencia(int id)
    {
        var ocorrencia = _context.Ocorrencias
            .Include(ocorrencia => ocorrencia.Categoria)
            .Include(ocorrencia => ocorrencia.Utilizador)
            .Include(ocorrencia => ocorrencia.OcorrenciaImpactos)
                .ThenInclude(ocorrenciaImpacto => ocorrenciaImpacto.Impacto)
            .Include(ocorrencia => ocorrencia.Comentarios)
                .ThenInclude(comentario => comentario.Utilizador)
            .Where(ocorrencia => ocorrencia.Id == id)
            .Select(ocorrencia => new
            {
                ocorrencia.Id,
                ocorrencia.Titulo,
                ocorrencia.Descricao,
                ocorrencia.LocalizacaoTexto,
                ocorrencia.Edificio,
                ocorrencia.Estado,
                ocorrencia.Prioridade,
                ocorrencia.NumeroApoios,
                DataCriacao = ocorrencia.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                Categoria = ocorrencia.Categoria != null
                    ? ocorrencia.Categoria.Nome
                    : null,
                ReportadaPor = ocorrencia.Utilizador != null
                    ? ocorrencia.Utilizador.Email
                    : "Utilizador não identificado",
                Impactos = ocorrencia.OcorrenciaImpactos
                    .Select(ocorrenciaImpacto => ocorrenciaImpacto.Impacto != null
                        ? ocorrenciaImpacto.Impacto.Nome
                        : "")
                    .Where(nome => nome != "")
                    .ToList(),
                Comentarios = ocorrencia.Comentarios
                    .OrderByDescending(comentario => comentario.DataCriacao)
                    .Select(comentario => new
                    {
                        comentario.Texto,
                        DataCriacao = comentario.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                        Utilizador = comentario.Utilizador != null
                            ? comentario.Utilizador.Email
                            : "Utilizador removido"
                    })
                    .ToList()
            })
            .FirstOrDefault();

        if (ocorrencia == null)
        {
            return NotFound();
        }

        return Ok(ocorrencia);
    }
}