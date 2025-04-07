using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Services;
using CmCapitalDevSrProject.Services.Models.DTOs;
using CmCapitalDevSrProject.Tests.Fakes;
using FluentAssertions;

namespace CmCapitalDevSrProject.Tests;

public class ClienteServiceTests
{
    [Fact]
    public async Task CriarCliente_DeveRetornarCliente_QuandoSaldoValido()
    {
        // Arrange
        var repository = new FakeClienteRepository();
        var service = new ClienteService(repository);

        var clienteDto = new ClienteDto
        {
            Nome = "Magno",
            SaldoDisponivel = 100
        };

        // Act
        var cliente = await service.CriarClienteAsync(clienteDto);

        // Assert
        cliente.Should().NotBeNull();
        cliente.Id.Should().Be(1);
        cliente.Nome.Should().Be("Magno");
        cliente.SaldoDisponivel.Should().Be(100);
    }

    [Fact]
    public async Task CriarCliente_DeveLancarExcecao_QuandoSaldoNegativo()
    {
        // Arrange
        var repository = new FakeClienteRepository();
        var service = new ClienteService(repository);

        var clienteDto = new ClienteDto
        {
            Nome = "Invalido",
            SaldoDisponivel = -50
        };

        // Act
        var act = async () => await service.CriarClienteAsync(clienteDto);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Saldo inicial não pode ser menor que zero.");
    }
}