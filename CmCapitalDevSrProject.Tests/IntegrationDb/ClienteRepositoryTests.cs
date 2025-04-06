using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Repositories;
using CmCapitalDevSrProject.Repositories.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CmCapitalDevSrProject.Tests.IntegrationDb;

public class ClienteRepositoryTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_DevePersistirCliente()
    {
        // Arrange
        var context = GetInMemoryDb();
        var repository = new ClienteRepository(context);

        var cliente = new Cliente
        {
            Nome = "Cliente Corretora",
            SaldoDisponivel = 500
        };

        // Act
        var result = await repository.AddAsync(cliente);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        var saved = await context.Clientes.FindAsync(result.Id);
        saved.Should().NotBeNull();
        saved?.Nome.Should().Be("Cliente Corretora");
    }
}