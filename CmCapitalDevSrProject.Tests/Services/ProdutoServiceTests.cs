using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Repositories;
using CmCapitalDevSrProject.Services;
using CmCapitalDevSrProject.Tests.Fakes;
using FluentAssertions;

namespace CmCapitalDevSrProject.Tests.Services;

public class ProdutoServiceTests
{
    [Fact]
    public async Task CriarProduto_DeveRetornarProduto_QuandoValido()
    {
        // Arrange
        var produtoRepository = new FakeProdutoRepository();
        var categoriaRepository = new FakeCategoriaProdutoRepository();
        var service = new ProdutoService(produtoRepository,categoriaRepository);

        var dto = new ProdutoDto
        {
            Nome = "Notebook",
            Preco = 2500,
            QuantidadeEstoque = 10,
            CategoriaId = 1,
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
        var produtoRepository = new FakeProdutoRepository();
        var categoriaRepository = new FakeCategoriaProdutoRepository();
        var service = new ProdutoService(produtoRepository,categoriaRepository);

        var dto = new ProdutoDto
        {
            Nome = "TV",
            Preco = -100,
            QuantidadeEstoque = 1,
            CategoriaId = 1,
            DataVencimento = DateTime.Now.AddDays(10)
        };

        var act = async () => await service.CriarProdutoAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Preço não pode ser negativo");
    }

    [Fact]
    public async Task CriarProduto_DeveLancarExcecao_QuandoDataVencida()
    {
        var produtoRepository = new FakeProdutoRepository();
        var categoriaRepository = new FakeCategoriaProdutoRepository();
        var service = new ProdutoService(produtoRepository,categoriaRepository);

        var dto = new ProdutoDto
        {
            Nome = "Produto Vencido",
            Preco = 50,
            QuantidadeEstoque = 5,
            CategoriaId = 1,
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
        var produtoRepository = new FakeProdutoRepository();
        var categoriaRepository = new FakeCategoriaProdutoRepository();
        var service = new ProdutoService(produtoRepository,categoriaRepository);

        produtoRepository.Produtos.AddRange(new[]
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
        var produtoRepository = new FakeProdutoRepository();
        var categoriaRepository = new FakeCategoriaProdutoRepository();
        var service = new ProdutoService(produtoRepository,categoriaRepository);

        produtoRepository.Produtos.AddRange(new[]
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
    public async Task SugerirProdutosAsync_DeveRetornarProdutosValidos()
    {
        // Arrange
        var produtoRepository = new FakeProdutoRepository();
        var clienteRepository = new FakeClienteRepository();
        var categoriaRepository = new FakeCategoriaProdutoRepository();
        

        var categoria = new CategoriaProduto { Id = 1, Nome = "Renda Fixa" };

        var produtoBase = new Produto
        {
            Nome = "Base",
            Preco = 1000,
            DataVencimento = DateTime.Today.AddMonths(6),
            Categoria = categoria,
            CategoriaId = categoria.Id
        };
        await produtoRepository.AddAsync(produtoBase);

        // Produtos válidos (mesma categoria, vencimento <= base - 4 meses, preço <= saldo)
        await produtoRepository.AddAsync(new Produto
        {
            Nome = "Produto 1",
            Preco = 300,
            DataVencimento = DateTime.Today.AddMonths(1),
            Categoria = categoria,
            CategoriaId = categoria.Id
        });

        await produtoRepository.AddAsync(new Produto
        {
            Nome = "Produto 2",
            Preco = 200,
            DataVencimento = DateTime.Today.AddMonths(2),
            Categoria = categoria,
            CategoriaId = categoria.Id
        });

        // Inválido: vencimento > base - 4 meses
        await produtoRepository.AddAsync(new Produto
        {
            Nome = "Produto fora do prazo",
            Preco = 100,
            DataVencimento = DateTime.Today.AddMonths(5),
            Categoria = categoria,
            CategoriaId = categoria.Id
        });

        // Inválido: outra categoria
        await produtoRepository.AddAsync(new Produto
        {
            Nome = "Produto outra categoria",
            Preco = 150,
            DataVencimento = DateTime.Today.AddMonths(1),
            Categoria = new CategoriaProduto { Id = 2, Nome = "Ações" },
            CategoriaId = 2
        });

        var cliente = new Cliente
        {
            Nome = "Cliente Teste",
            SaldoDisponivel = 400
        };
        await clienteRepository.AddAsync(cliente);

        var service = new ProdutoService(produtoRepository, categoriaRepository);

        // Act
        var sugestoes = await service.SugerirProdutosAsync(cliente.Id, produtoBase.Id, clienteRepository);

        // Assert
        Assert.Equal(2, sugestoes.Count);
        Assert.Contains(sugestoes, s => s.Nome == "Produto 1");
        Assert.Contains(sugestoes, s => s.Nome == "Produto 2");
        Assert.DoesNotContain(sugestoes, s => s.Nome == "Produto fora do prazo");
        Assert.DoesNotContain(sugestoes, s => s.Nome == "Produto outra categoria");
    }




}
