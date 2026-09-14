using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.DTOs;
using PortalOcorrenciasIPT.Models;
using Microsoft.AspNetCore.SignalR;
using PortalOcorrenciasIPT.Hubs;

namespace PortalOcorrenciasIPT.Controllers;

[ApiController]
[Route("api/ocorrencias")]
public class OcorrenciasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly IHubContext<OcorrenciasHub> _hubContext;

    private static readonly string[] EstadosValidos =
{
    "Aberta",
    "Em análise",
    "Em resolução",
    "Resolvida",
    "Encerrada"
};

    private static readonly string[] PrioridadesValidas =
    {
    "Baixa",
    "Normal",
    "Alta",
    "Urgente"
};

    public OcorrenciasController(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IHubContext<OcorrenciasHub> hubContext)
    {
        _context = context;
        _userManager = userManager;
        _hubContext = hubContext;
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
    [HttpPost("{id}/apoiar")]
    public IActionResult ApoiarOcorrencia(int id)
    {
        var ocorrencia = _context.Ocorrencias
            .FirstOrDefault(ocorrencia => ocorrencia.Id == id);

        if (ocorrencia == null)
        {
            return NotFound();
        }

        ocorrencia.NumeroApoios++;

        _context.SaveChanges();

        return Ok(new
        {
            OcorrenciaId = ocorrencia.Id,
            NumeroApoios = ocorrencia.NumeroApoios,
            Mensagem = "Apoio registado com sucesso."
        });
    }
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public async Task<IActionResult> CriarOcorrencia([FromBody] CriarOcorrenciaDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ApplicationUser? utilizadorAtual = await _userManager.GetUserAsync(User);

        if (utilizadorAtual == null)
        {
            return Unauthorized();
        }

        bool categoriaExiste = _context.Categorias
            .Any(categoria => categoria.Id == dto.CategoriaId && categoria.Ativa);

        if (!categoriaExiste)
        {
            return BadRequest(new
            {
                Mensagem = "A categoria indicada não existe ou não está ativa."
            });
        }

        List<int> impactosValidosIds = _context.Impactos
            .Where(impacto => impacto.Ativo && dto.ImpactosIds.Contains(impacto.Id))
            .Select(impacto => impacto.Id)
            .ToList();

        Ocorrencia ocorrencia = new Ocorrencia
        {
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            LocalizacaoTexto = dto.LocalizacaoTexto,
            Edificio = dto.Edificio,
            CategoriaId = dto.CategoriaId,
            Prioridade = dto.Prioridade,
            Estado = "Aberta",
            DataCriacao = DateTime.Now,
            NumeroApoios = 0,
            UtilizadorId = utilizadorAtual.Id
        };

        _context.Ocorrencias.Add(ocorrencia);
        _context.SaveChanges();

        foreach (int impactoId in impactosValidosIds)
        {
            OcorrenciaImpacto ocorrenciaImpacto = new OcorrenciaImpacto
            {
                OcorrenciaId = ocorrencia.Id,
                ImpactoId = impactoId
            };

            _context.OcorrenciaImpactos.Add(ocorrenciaImpacto);
        }

        _context.SaveChanges();

        await _hubContext.Clients.All.SendAsync(
            "NovaOcorrencia",
            ocorrencia.Titulo,
            ocorrencia.LocalizacaoTexto);

        return CreatedAtAction(
            nameof(GetOcorrencia),
            new { id = ocorrencia.Id },
            new
            {
                ocorrencia.Id,
                ocorrencia.Titulo,
                ocorrencia.Estado,
                ocorrencia.Prioridade,
                ocorrencia.NumeroApoios,
                Mensagem = "Ocorrência criada com sucesso."
            });
    }

    [Authorize(
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    Roles = "Gestor")]
    [HttpPut("{id}/gestao")]
    public IActionResult AtualizarGestaoOcorrencia(
    int id,
    [FromBody] AtualizarGestaoOcorrenciaDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!EstadosValidos.Contains(dto.Estado))
        {
            return BadRequest(new
            {
                Mensagem = "Estado inválido."
            });
        }

        if (!PrioridadesValidas.Contains(dto.Prioridade))
        {
            return BadRequest(new
            {
                Mensagem = "Prioridade inválida."
            });
        }

        Ocorrencia? ocorrencia = _context.Ocorrencias
            .FirstOrDefault(ocorrencia => ocorrencia.Id == id);

        if (ocorrencia == null)
        {
            return NotFound(new
            {
                Mensagem = "Ocorrência não encontrada."
            });
        }

        ocorrencia.Estado = dto.Estado;
        ocorrencia.Prioridade = dto.Prioridade;

        _context.SaveChanges();

        return Ok(new
        {
            ocorrencia.Id,
            ocorrencia.Titulo,
            ocorrencia.Estado,
            ocorrencia.Prioridade,
            Mensagem = "Ocorrência atualizada com sucesso."
        });
    }

    [Authorize(
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    Roles = "Gestor")]
    [HttpDelete("{id}")]
    public IActionResult EliminarOcorrencia(int id)
    {
        Ocorrencia? ocorrencia = _context.Ocorrencias
            .FirstOrDefault(ocorrencia => ocorrencia.Id == id);

        if (ocorrencia == null)
        {
            return NotFound(new
            {
                Mensagem = "Ocorrência não encontrada."
            });
        }

        _context.Ocorrencias.Remove(ocorrencia);
        _context.SaveChanges();

        return Ok(new
        {
            ocorrencia.Id,
            ocorrencia.Titulo,
            Mensagem = "Ocorrência eliminada com sucesso."
        });
    }
}