using CmCapitalDevSrProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CmCapitalDevSrProject.Repositories.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) {}

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<ProdutoAuditoria> ProdutosAuditoria => Set<ProdutoAuditoria>();
    
    public DbSet<CategoriaProduto> Categorias => Set<CategoriaProduto>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entidadesModificadas = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified && e.Entity is EntidadeBase);

        foreach (var entry in entidadesModificadas)
        {
            ((EntidadeBase)entry.Entity).UpdatedAt = DateTime.Now;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(EntidadeBase).IsAssignableFrom(entityType.ClrType)) continue;
            modelBuilder.Entity(entityType.ClrType)
                .Property(nameof(EntidadeBase.CreatedAt))
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity(entityType.ClrType)
                .Property(nameof(EntidadeBase.UpdatedAt))
                .IsRequired(false);
        }
    }
}