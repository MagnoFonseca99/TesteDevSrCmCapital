using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Services;
using CmCapitalDevSrProject.Services.Models.DTOs;
using CmCapitalDevSrProject.Tests.Fakes;

namespace CmCapitalDevSrProject.Tests.Services;

public class VendaServiceTests
{
    private readonly FakeClienteRepository _clienteRepo;
    private readonly FakeProdutoRepository _produtoRepo;
    private readonly FakeVendaRepository _vendaRepo;
    private readonly ProdutoService _produtoService;
    private readonly VendaService _vendaService;

    public VendaServiceTests()
    {
        _clienteRepo = new FakeClienteRepository();
        _produtoRepo = new FakeProdutoRepository();
        _produtoService = new ProdutoService(_produtoRepo, new FakeCategoriaProdutoRepository());
        _vendaRepo = new FakeVendaRepository();

        _vendaService = new VendaService(
            _clienteRepo,
            _produtoRepo,
            _produtoService,
            _vendaRepo
        );
    }

    [Fact]
    public async Task RealizarVendaAsync_DeveRetornarVendaResultado_SeTudoEstiverValido()
    {
        var cliente = new Cliente { Nome = "João", SaldoDisponivel = 1000 };
        var produto = new Produto { Nome = "Produto A", Preco = 100, QuantidadeEstoque = 10, DataVencimento = DateTime.Now.AddMonths(6) };

        await _clienteRepo.AddAsync(cliente);
        await _produtoRepo.AddAsync(produto);

        var dto = new VendaDto
        {
            ClienteId = cliente.Id,
            ProdutoId = produto.Id,
            Quantidade = 2
        };

        var resultado = await _vendaService.RealizarVendaAsync(dto, saldoMinimoPermitido: 100);

        Assert.Equal(cliente.Nome, resultado.Cliente);
        Assert.Equal(produto.Nome, resultado.Produto);
        Assert.Equal(200, resultado.ValorTotal);
        Assert.Equal(1, resultado.VendaId);
    }

    [Fact]
    public async Task RealizarVendaAsync_DeveLancarExcecao_SeClienteNaoExistir()
    {
        var dto = new VendaDto { ClienteId = 999, ProdutoId = 1, Quantidade = 1 };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _vendaService.RealizarVendaAsync(dto, 0));

        Assert.Equal("Cliente não encontrado", ex.Message);
    }

    [Fact]
    public async Task RealizarVendaAsync_DeveLancarExcecao_SeProdutoNaoExistir()
    {
        var cliente = new Cliente { Nome = "João", SaldoDisponivel = 1000 };
        await _clienteRepo.AddAsync(cliente);

        var dto = new VendaDto { ClienteId = cliente.Id, ProdutoId = 999, Quantidade = 1 };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _vendaService.RealizarVendaAsync(dto, 0));

        Assert.Equal("Produto não encontrado", ex.Message);
    }

    [Fact]
    public async Task RealizarVendaAsync_DeveLancarExcecao_SeQuantidadeZero()
    {
        var cliente = new Cliente { Nome = "João", SaldoDisponivel = 1000 };
        var produto = new Produto { Nome = "Produto A", Preco = 100, QuantidadeEstoque = 10, DataVencimento = DateTime.Now.AddMonths(6) };

        await _clienteRepo.AddAsync(cliente);
        await _produtoRepo.AddAsync(produto);

        var dto = new VendaDto
        {
            ClienteId = cliente.Id,
            ProdutoId = produto.Id,
            Quantidade = 0
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _vendaService.RealizarVendaAsync(dto, 0));

        Assert.Equal("Quantidade deve ser maior que zero", ex.Message);
    }

    [Fact]
    public async Task RealizarVendaAsync_DeveLancarExcecao_SeEstoqueInsuficiente()
    {
        var cliente = new Cliente { Nome = "João", SaldoDisponivel = 1000 };
        var produto = new Produto { Nome = "Produto A", Preco = 100, QuantidadeEstoque = 1, DataVencimento = DateTime.Now.AddMonths(6) };

        await _clienteRepo.AddAsync(cliente);
        await _produtoRepo.AddAsync(produto);

        var dto = new VendaDto
        {
            ClienteId = cliente.Id,
            ProdutoId = produto.Id,
            Quantidade = 2
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _vendaService.RealizarVendaAsync(dto, 0));

        Assert.Equal("Estoque insuficiente", ex.Message);
    }

    [Fact]
    public async Task RealizarVendaAsync_DeveLancarExcecao_SeSaldoFicarAbaixoDoPermitido()
    {
        var cliente = new Cliente { Nome = "João", SaldoDisponivel = 200 };
        var produto = new Produto { Nome = "Produto A", Preco = 100, QuantidadeEstoque = 10, DataVencimento = DateTime.Now.AddMonths(6) };

        await _clienteRepo.AddAsync(cliente);
        await _produtoRepo.AddAsync(produto);

        var dto = new VendaDto
        {
            ClienteId = cliente.Id,
            ProdutoId = produto.Id,
            Quantidade = 2
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _vendaService.RealizarVendaAsync(dto, saldoMinimoPermitido: 50));

        Assert.Equal("Saldo insuficiente para a compra", ex.Message);
    }

    [Fact]
    public async Task RealizarEstornoAsync_DeveLancarExcecao_SeVendaNaoExistir()
    {
        var dto = new EstornoVendaDto { VendaId = 999, ClienteId = 1 };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _vendaService.RealizarEstornoAsync(dto));

        Assert.Equal("Venda não encontrada.", ex.Message);
    }

    [Fact]
    public async Task RealizarEstornoAsync_DeveEstornarVenda_SeDentroDoPrazo()
    {
        // Arrange
        var cliente = new Cliente { Nome = "Maria", SaldoDisponivel = 1000 };
        var produto = new Produto
        {
            Nome = "Produto B",
            Preco = 200,
            QuantidadeEstoque = 5,
            DataVencimento = DateTime.Now.AddMonths(6)
        };

        await _clienteRepo.AddAsync(cliente);
        await _produtoRepo.AddAsync(produto);

        var venda = new Venda
        {
            ClienteId = cliente.Id,
            ProdutoId = produto.Id,
            Quantidade = 2,
            ValorTotal = 400,
            DataVenda = DateTime.UtcNow.AddDays(-3)
        };

        await _vendaRepo.RegistrarVendaAsync(venda);

        // Simula o efeito da venda no cliente e produto
        var clienteAntes = await _clienteRepo.GetByIdAsync(cliente.Id);
        clienteAntes.SaldoDisponivel -= venda.ValorTotal;

        var produtoAntes = await _produtoRepo.GetByIdAsync(produto.Id);
        produtoAntes.QuantidadeEstoque -= venda.Quantidade;

        var dto = new EstornoVendaDto
        {
            VendaId = venda.Id,
            ClienteId = cliente.Id
        };

        // Act
        await _vendaService.RealizarEstornoAsync(dto);

        // Assert
        var vendaEstornada = _vendaRepo.ObterVendasRegistradas().First(v => v.Id == venda.Id);
        Assert.True(vendaEstornada.Estornada);
        Assert.NotNull(vendaEstornada.DataEstorno);

        var clienteAtualizado = await _clienteRepo.GetByIdAsync(cliente.Id);
        Assert.Equal(1000, clienteAtualizado?.SaldoDisponivel); // 1000 original

        var produtoAtualizado = await _produtoRepo.GetByIdAsync(produto.Id);
        Assert.Equal(5, produtoAtualizado?.QuantidadeEstoque); // voltou ao estoque original
    }

    [Fact]
    public async Task RealizarEstornoAsync_DeveLancarExcecao_SeVendaJaEstornada()
    {
        var cliente = new Cliente { Nome = "Ana", SaldoDisponivel = 1000 };
        var produto = new Produto { Nome = "Produto D", Preco = 100, QuantidadeEstoque = 5, DataVencimento = DateTime.Now.AddMonths(6) };

        await _clienteRepo.AddAsync(cliente);
        await _produtoRepo.AddAsync(produto);

        var venda = new Venda
        {
            ClienteId = cliente.Id,
            ProdutoId = produto.Id,
            Quantidade = 1,
            ValorTotal = 100,
            DataVenda = DateTime.UtcNow.AddDays(-2),
            Estornada = true
        };

        await _vendaRepo.RegistrarVendaAsync(venda);

        var dto = new EstornoVendaDto { VendaId = venda.Id, ClienteId = cliente.Id };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _vendaService.RealizarEstornoAsync(dto));

        Assert.Equal("Venda já foi estornada.", ex.Message);
    }

    [Fact]
    public async Task RealizarEstornoAsync_DeveLancarExcecao_SeVendaForAntiga()
    {
        var cliente = new Cliente { Nome = "Pedro", SaldoDisponivel = 1000 };
        var produto = new Produto { Nome = "Produto E", Preco = 300, QuantidadeEstoque = 3, DataVencimento = DateTime.Now.AddMonths(6) };

        await _clienteRepo.AddAsync(cliente);
        await _produtoRepo.AddAsync(produto);

        var venda = new Venda
        {
            ClienteId = cliente.Id,
            ProdutoId = produto.Id,
            Quantidade = 1,
            ValorTotal = 300,
            DataVenda = DateTime.UtcNow.AddDays(-10)
        };

        await _vendaRepo.RegistrarVendaAsync(venda);

        var dto = new EstornoVendaDto { VendaId = venda.Id, ClienteId = cliente.Id };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _vendaService.RealizarEstornoAsync(dto));

        Assert.Equal("Prazo de estorno expirado.", ex.Message);
    }
}
