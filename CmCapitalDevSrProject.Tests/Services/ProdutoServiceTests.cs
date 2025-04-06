using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Repositories;
using CmCapitalDevSrProject.Services;
using CmCapitalDevSrProject.Tests.Repositories;
using FluentAssertions;

namespace CmCapitalDevSrProject.Tests.Services;

public class ProdutoServiceTests
{
    [Fact]
    public async Task CriarProduto_DeveRetornarProduto_QuandoValido()
    {
        // Arrange
        var repository = new FakeProdutoRepository();
        var service = new ProdutoService(repository);

        var dto = new ProdutoDto
        {
            Nome = "Notebook",
            Preco = 2500,
            QuantidadeEstoque = 10,
            DataVencimento = DateTime.Now.AddDays(30)
        };

        // Act
        var produto = await service.CriarProdutoAsync(dto);

        // Assert
        produto.Should().NotBeNull();
        produto.Id.Should().Be(1);
        produto.Nome.Should().Be("Notebook");
    }

    [Fact]
    public async Task CriarProduto_DeveLancarExcecao_QuandoPrecoNegativo()
    {
        var repository = new FakeProdutoRepository();
        var service = new ProdutoService(repository);

        var dto = new ProdutoDto
        {
            Nome = "TV",
            Preco = -100,
            QuantidadeEstoque = 1,
            DataVencimento = DateTime.Now.AddDays(10)
        };

        var act = async () => await service.CriarProdutoAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Preço não pode ser negativo");
    }

    [Fact]
    public async Task CriarProduto_DeveLancarExcecao_QuandoDataVencida()
    {
        var repository = new FakeProdutoRepository();
        var service = new ProdutoService(repository);

        var dto = new ProdutoDto
        {
            Nome = "Produto Vencido",
            Preco = 50,
            QuantidadeEstoque = 5,
            DataVencimento = DateTime.Now.AddDays(-1)
        };

        var act = async () => await service.CriarProdutoAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Data de vencimento deve ser no futuro");
    }
    
    [Fact]
    public async Task BuscarProdutos_DeveFiltrarPorPrecoEEstoque()
    {
        // Arrange
        var repository = new FakeProdutoRepository();
        var service = new ProdutoService(repository);

        repository.Produtos.AddRange(new[]
        {
            new Produto
            {
                Id = 1,
                Nome = "Produto A",
                Preco = 100,
                QuantidadeEstoque = 10,
                CategoriaId = 1,
                DataVencimento = DateTime.Now.AddDays(30),
                Categoria = new CategoriaProduto { Id = 1, Nome = "Categoria 1" }
            },
            new Produto
            {
                Id = 2,
                Nome = "Produto B",
                Preco = 200,
                QuantidadeEstoque = 3,
                CategoriaId = 2,
                DataVencimento = DateTime.Now.AddDays(20),
                Categoria = new CategoriaProduto { Id = 2, Nome = "Categoria 2" }
            },
            new Produto
            {
                Id = 3,
                Nome = "Produto C",
                Preco = 80,
                CategoriaId = 3,
                QuantidadeEstoque = 15,
                DataVencimento = DateTime.Now.AddDays(5),
                Categoria = new CategoriaProduto { Id = 3, Nome = "Categoria 3" }
            }
        });

        var filtro = new ProdutoFiltroDto
        {
            PrecoMin = 90,
            PrecoMax = 200,
            QuantidadeMin = 5,
            Page = 1,
            PageSize = 10
        };

        // Act
        var resultado = await service.BuscarProdutosAsync(filtro);

        // Assert
        resultado.Should().NotBeNull();
        resultado.TotalItems.Should().Be(1);
        resultado.Items.Should().ContainSingle();
        resultado.Items[0].Nome.Should().Be("Produto A");
    }
    
    [Fact]
    public async Task ObterProdutosComEstoqueBaixoAsync_DeveRetornarApenasProdutosCriticos()
    {
        // Arrange
        var repository = new FakeProdutoRepository();
        var service = new ProdutoService(repository);

        repository.Produtos.AddRange(new[]
        {
            new Produto
            {
                Id = 1,
                Nome = "Produto A",
                Preco = 100,
                QuantidadeEstoque = 10,
                CategoriaId = 1,
                DataVencimento = DateTime.Now.AddDays(30),
                Categoria = new CategoriaProduto { Id = 1, Nome = "Categoria 1" }
            },
            new Produto
            {
                Id = 2,
                Nome = "Produto B",
                Preco = 200,
                QuantidadeEstoque = 3,
                CategoriaId = 2,
                DataVencimento = DateTime.Now.AddDays(20),
                Categoria = new CategoriaProduto { Id = 2, Nome = "Categoria 2" }
            },
            new Produto
            {
                Id = 3,
                Nome = "Produto C",
                Preco = 80,
                CategoriaId = 3,
                QuantidadeEstoque = 4,
                DataVencimento = DateTime.Now.AddDays(5),
                Categoria = new CategoriaProduto { Id = 3, Nome = "Categoria 3" }
            }
        });

        int limiteMinimo = 5;

        // Act
        var resultado = await service.ObterProdutosComEstoqueBaixoAsync(limiteMinimo);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.Select(p => p.Nome).Should().BeEquivalentTo("Produto B", "Produto C");
    }
    
    [Fact]
    public async Task SugerirProdutosAsync_DeveRetornarSugestoesDaMesmaCategoriaDentroDoSaldoEVencimento()
    {
        // Arrange
        var produtoRepository = new FakeProdutoRepository();
        var clienteRepository = new FakeClienteRepository();

        var categoriaId = 1;
        var dataBase = DateTime.Now.AddMonths(8);

        // Produto base
        var produtoBase = new Produto
        {
            Id = 100,
            Nome = "Produto Base",
            Preco = 1000,
            CategoriaId = categoriaId,
            QuantidadeEstoque = 5,
            DataVencimento = dataBase,
            Categoria = new CategoriaProduto { Id = categoriaId, Nome = "Renda Fixa" }
        };

        // Sugestões válidas e inválidas
        var sugestoes = new List<Produto>
        {
            new Produto
            {
                Id = 1,
                Nome = "Sugestão A",
                Preco = 300,
                CategoriaId = categoriaId,
                DataVencimento = dataBase.AddMonths(-5),
                Categoria = new CategoriaProduto { Id = categoriaId, Nome = "Renda Fixa" }
            },
            new Produto
            {
                Id = 2,
                Nome = "Sugestão B",
                Preco = 500,
                CategoriaId = categoriaId,
                DataVencimento = dataBase.AddMonths(-4),
                Categoria = new CategoriaProduto { Id = categoriaId, Nome = "Renda Fixa" }
            },
            new Produto
            {
                Id = 5,
                Nome = "Fora do Vencimento",
                Preco = 600,
                CategoriaId = categoriaId,
                DataVencimento = dataBase.AddMonths(-3), // inválido: muito próximo do vencimento
                Categoria = new CategoriaProduto { Id = categoriaId, Nome = "Renda Fixa" }
            }
        };

        // Adiciona os produtos ao repositório fake
        produtoRepository.Produtos.Add(produtoBase);
        produtoRepository.Produtos.AddRange(sugestoes);

        // Cliente com saldo disponível
        clienteRepository.Clientes.Add(new Cliente
        {
            Id = 1,
            Nome = "Cliente",
            SaldoDisponivel = 600
        });

        var service = new ProdutoService(produtoRepository);

        // Act
        var resultado = await service.SugerirProdutosAsync(1, 100, clienteRepository);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Select(r => r.Nome).Should().BeEquivalentTo("Sugestão A", "Sugestão B");
    }




}
