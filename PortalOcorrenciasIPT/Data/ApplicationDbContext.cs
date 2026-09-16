using Microsoft.EntityFrameworkCore;
using PortalOcorrenciasIPT.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PortalOcorrenciasIPT.Data;

// O contexto da aplicação herda de IdentityDbContext<ApplicationUser>,
// permitindo usar as tabelas do ASP.NET Identity em conjunto com as tabelas próprias do projeto.
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Tabelas principais da aplicação.
    public DbSet<Categoria> Categorias { get; set; }

    public DbSet<Ocorrencia> Ocorrencias { get; set; }

    public DbSet<Impacto> Impactos { get; set; }

    public DbSet<OcorrenciaImpacto> OcorrenciaImpactos { get; set; }

    public DbSet<Comentario> Comentarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mantém toda a configuração interna necessária ao ASP.NET Identity
        // antes de acrescentar as configurações específicas da aplicação.
        base.OnModelCreating(modelBuilder);

        // Relação muitos-para-muitos entre Ocorrencia e Impacto.
        // A tabela de junção OcorrenciaImpacto usa uma chave primária composta,
        // garantindo que o mesmo impacto não é repetido na mesma ocorrência.
        modelBuilder.Entity<OcorrenciaImpacto>()
            .HasKey(oi => new { oi.OcorrenciaId, oi.ImpactoId });

        // Uma ocorrência pode ter vários impactos associados.
        modelBuilder.Entity<OcorrenciaImpacto>()
            .HasOne(oi => oi.Ocorrencia)
            .WithMany(o => o.OcorrenciaImpactos)
            .HasForeignKey(oi => oi.OcorrenciaId);

        // Um impacto pode ser associado a várias ocorrências.
        modelBuilder.Entity<OcorrenciaImpacto>()
            .HasOne(oi => oi.Impacto)
            .WithMany(i => i.OcorrenciaImpactos)
            .HasForeignKey(oi => oi.ImpactoId);

        // Cada ocorrência pode ficar associada ao utilizador que a criou.
        // Se o utilizador for removido, a ocorrência é preservada e o UtilizadorId fica a null.
        modelBuilder.Entity<Ocorrencia>()
            .HasOne(o => o.Utilizador)
            .WithMany()
            .HasForeignKey(o => o.UtilizadorId)
            .OnDelete(DeleteBehavior.SetNull);

        // Dados iniciais para a tabela Impactos.
        // Estes valores permitem demonstrar a relação N:N logo após a criação da base de dados.
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

        // Uma ocorrência pode ter vários comentários.
        // Ao eliminar uma ocorrência, os seus comentários são também eliminados,
        // porque deixam de fazer sentido sem a ocorrência associada.
        modelBuilder.Entity<Comentario>()
            .HasOne(c => c.Ocorrencia)
            .WithMany(o => o.Comentarios)
            .HasForeignKey(c => c.OcorrenciaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cada comentário pertence a um utilizador autenticado.
        // O comportamento Restrict impede apagar utilizadores que ainda tenham comentários associados,
        // preservando a autoria e a integridade dos dados.
        modelBuilder.Entity<Comentario>()
            .HasOne(c => c.Utilizador)
            .WithMany()
            .HasForeignKey(c => c.UtilizadorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}