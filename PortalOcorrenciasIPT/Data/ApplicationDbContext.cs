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
}
