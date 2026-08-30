using Microsoft.EntityFrameworkCore;
using PortalOcorrenciasIPT.Models;

namespace PortalOcorrenciasIPT.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Categoria> Categorias { get; set; }

    public DbSet<Ocorrencia> Ocorrencias { get; set; }

    public DbSet<Impacto> Impactos { get; set; }

    public DbSet<OcorrenciaImpacto> OcorrenciaImpactos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OcorrenciaImpacto>()
            .HasKey(oi => new { oi.OcorrenciaId, oi.ImpactoId });

        modelBuilder.Entity<OcorrenciaImpacto>()
            .HasOne(oi => oi.Ocorrencia)
            .WithMany(o => o.OcorrenciaImpactos)
            .HasForeignKey(oi => oi.OcorrenciaId);

        modelBuilder.Entity<OcorrenciaImpacto>()
            .HasOne(oi => oi.Impacto)
            .WithMany(i => i.OcorrenciaImpactos)
            .HasForeignKey(oi => oi.ImpactoId);

        modelBuilder.Entity<Impacto>().HasData(
            new Impacto
            {
                Id = 1,
                Nome = "Afeta várias pessoas",
                Descricao = "A ocorrência tem impacto em mais do que um utilizador ou grupo."
            },
            new Impacto
            {
                Id = 2,
                Nome = "Impede aulas",
                Descricao = "A ocorrência impede ou dificulta a realização de aulas."
            },
            new Impacto
            {
                Id = 3,
                Nome = "Risco de segurança",
                Descricao = "A ocorrência pode colocar pessoas ou bens em risco."
            },
            new Impacto
            {
                Id = 4,
                Nome = "Acesso bloqueado",
                Descricao = "A ocorrência impede ou dificulta o acesso a um espaço."
            },
            new Impacto
            {
                Id = 5,
                Nome = "Problema recorrente",
                Descricao = "A ocorrência já aconteceu anteriormente."
            },
            new Impacto
            {
                Id = 6,
                Nome = "Afeta equipamento essencial",
                Descricao = "A ocorrência envolve equipamento necessário ao funcionamento normal das atividades."
            }
        );
    }
}