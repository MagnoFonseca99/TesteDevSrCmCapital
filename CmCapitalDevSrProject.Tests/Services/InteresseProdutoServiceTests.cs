using System;
using System.Threading.Tasks;
using Xunit;
using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Services;
using CmCapitalDevSrProject.Tests.Fakes;
using CmCapitalDevSrProject.Repositories.Interfaces;
using CmCapitalDevSrProject.Services.Models.DTOs;

namespace CmCapitalDevSrProject.Tests.Services;

public class InteresseProdutoServiceTests
{
    private readonly FakeInteresseProdutoRepository _interesseRepo;
    private readonly FakeClienteRepository _clienteRepo;
    private readonly FakeCategoriaProdutoRepository _categoriaRepo;
    private readonly InteresseProdutoService _service;

    public InteresseProdutoServiceTests()
    {
        _interesseRepo = new FakeInteresseProdutoRepository();
        _clienteRepo = new FakeClienteRepository();
        _categoriaRepo = new FakeCategoriaProdutoRepository();

        _service = new InteresseProdutoService(_interesseRepo, _clienteRepo, _categoriaRepo);
    }

    [Fact]
    public async Task RegistrarInteresseAsync_DeveLancarExcecao_SeClienteNaoExistir()
    {
        var dto = new InteresseProdutoFuturoDto
        {
            ClienteId = 999,
            CategoriaId = null
        };

        var act = async () => await _service.RegistrarInteresseAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task RegistrarInteresseAsync_DeveLancarExcecao_SeCategoriaInformadaNaoExistir()
    {
        var cliente = new Cliente { Nome = "João", SaldoDisponivel = 500 };
        await _clienteRepo.AddAsync(cliente);

        var dto = new InteresseProdutoFuturoDto
        {
            ClienteId = cliente.Id,
            CategoriaId = 999
        };

        var act = async () => await _service.RegistrarInteresseAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task RegistrarInteresseAsync_DeveRegistrarComSucesso_SeCategoriaNaoInformada()
    {
        var cliente = new Cliente { Nome = "Maria", SaldoDisponivel = 700 };
        await _clienteRepo.AddAsync(cliente);

        var dto = new InteresseProdutoFuturoDto
        {
            ClienteId = cliente.Id,
            CategoriaId = null
        };

        await _service.RegistrarInteresseAsync(dto);

        var interesses = _interesseRepo.ObterTodos();
        Assert.Single(interesses);
        Assert.Equal(cliente.Id, interesses[0].ClienteId);
        Assert.Null(interesses[0].CategoriaId);
    }

    [Fact]
    public async Task RegistrarInteresseAsync_DeveRegistrarComSucesso_SeCategoriaInformadaEValida()
    {
        var cliente = new Cliente { Nome = "Lucas", SaldoDisponivel = 900 };
        var categoria = new CategoriaProduto { Id = 1, Nome = "Tesouro Direto" };

        await _clienteRepo.AddAsync(cliente);
        _categoriaRepo.Categorias.Add(categoria); // fake direto

        var dto = new InteresseProdutoFuturoDto
        {
            ClienteId = cliente.Id,
            CategoriaId = categoria.Id
        };

        await _service.RegistrarInteresseAsync(dto);

        var interesses = _interesseRepo.ObterTodos();
        Assert.Single(interesses);
        Assert.Equal(cliente.Id, interesses[0].ClienteId);
        Assert.Equal(categoria.Id, interesses[0].CategoriaId);
    }
}
