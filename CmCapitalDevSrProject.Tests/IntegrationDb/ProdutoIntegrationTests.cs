using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Repositories;
using CmCapitalDevSrProject.Repositories.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CmCapitalDevSrProject.Tests.IntegrationDb;

public class ProdutoIntegrationTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_DevePersistirProduto()
    {
        // Arrange
        var context = GetInMemoryDb();
        var repository = new ProdutoRepository(context);

        var produto = new Produto
        {
            Nome = "CDB",
            Preco = 80,
            QuantidadeEstoque = 25,
            CategoriaId = 1,
            DataVencimento = DateTime.Now.AddDays(90)
        };

        // Act
        var result = await repository.AddAsync(produto);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        var saved = await context.Produtos.FindAsync(result.Id);
        saved.Should().NotBeNull();
        saved?.Nome.Should().Be("CDB");
    }

    [Fact]
    public async Task AddAuditoriaAsync_DeveSalvarAuditoria()
    {
        // Arrange
        var context = GetInMemoryDb();
        var repository = new ProdutoRepository(context);

        var auditoria = new ProdutoAuditoria
        {
            ProdutoId = 1,
            Operacao = "CREATE",
            DadosNovos = "{}"
        };

        // Act
        await repository.AddAuditoriaAsync(auditoria);

        // Assert
        var saved = await context.ProdutosAuditoria.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.Operacao.Should().Be("CREATE");
    }
}