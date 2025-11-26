using Microsoft.EntityFrameworkCore;
using MenuManagementAPI.Domain.Entities;

namespace MenuManagementAPI.Infrastructure.Data;

/// <summary>
/// Contexto do banco de dados
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Menu> Menus { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Menu
        modelBuilder.Entity<Menu>(entity =>
        {
            entity.ToTable("Menus");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Ordem)
                .IsRequired();

            entity.Property(e => e.Icone)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Descricao)
                .HasMaxLength(1000);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.CriadoEm)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.AtualizadoEm)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Índice para busca por nome
            entity.HasIndex(e => e.Nome);
        });
    }
}
