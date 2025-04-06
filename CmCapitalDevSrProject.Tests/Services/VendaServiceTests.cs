using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Services;
using CmCapitalDevSrProject.Tests.Repositories;

namespace CmCapitalDevSrProject.Tests.Services
{
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
            _produtoService = new ProdutoService(_produtoRepo);
            _vendaRepo = new FakeVendaRepository();

            _vendaService = new VendaService(
                _clienteRepo,
                _produtoRepo,
                _produtoService,
                _vendaRepo,
                null // DbContext não é usado diretamente na service
            );
        }

        [Fact]
        public async Task RealizarVendaAsync_DeveRetornarVendaResultado_SeTudoEstiverValido()
        {
            // Arrange
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

            // Act
            var resultado = await _vendaService.RealizarVendaAsync(dto, saldoMinimoPermitido: 100);

            // Assert
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
    }
}
