using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Repositories.Data;
using CmCapitalDevSrProject.Repositories.Interfaces;
using CmCapitalDevSrProject.Services.Interfaces;

public class VendaService : IVendaService
{
    private readonly IClienteRepository _clienteRepo;
    private readonly IProdutoRepository _produtoRepo;
    private readonly IProdutoService _produtoService;
    private readonly IVendaRepository _vendaRepo;

    public VendaService(
        IClienteRepository clienteRepo,
        IProdutoRepository produtoRepo,
        IProdutoService produtoService,
        IVendaRepository vendaRepo,
        AppDbContext dbContext)
    {
        _clienteRepo = clienteRepo;
        _produtoRepo = produtoRepo;
        _produtoService = produtoService;
        _vendaRepo = vendaRepo;
    }

    public async Task<VendaResultadoDto> RealizarVendaAsync(VendaDto dto, decimal saldoMinimoPermitido)
    {
        try
        {
            var cliente = await _clienteRepo.GetByIdAsync(dto.ClienteId)
                          ?? throw new ArgumentException("Cliente não encontrado");

            var produto = await _produtoRepo.GetByIdAsync(dto.ProdutoId)
                          ?? throw new ArgumentException("Produto não encontrado");

            if (dto.Quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero");

            if (produto.QuantidadeEstoque < dto.Quantidade)
                throw new InvalidOperationException("Estoque insuficiente");

            var valorTotal = produto.Preco * dto.Quantidade;
            var saldoFinal = cliente.SaldoDisponivel - valorTotal;

            if (saldoFinal < saldoMinimoPermitido)
                throw new InvalidOperationException("Saldo insuficiente para a compra");

            cliente.SaldoDisponivel = saldoFinal;

            var produtoAtualizado = new Produto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Preco = produto.Preco,
                CategoriaId = produto.CategoriaId,
                DataVencimento = produto.DataVencimento,
                QuantidadeEstoque = produto.QuantidadeEstoque - dto.Quantidade
            };

            var venda = new Venda
            {
                ClienteId = cliente.Id,
                ProdutoId = produto.Id,
                Quantidade = dto.Quantidade,
                ValorTotal = valorTotal
            };

            await _produtoService.AtualizarProdutoComProdutoAnteriorAsync(produto.Id, produto, produtoAtualizado);
            await _clienteRepo.UpdateAsync(dto.ClienteId, cliente);

            var vendaEfetuada = await _vendaRepo.RegistrarVendaAsync(venda);

            return new VendaResultadoDto
            {
                VendaId = vendaEfetuada.Id,
                Cliente = cliente.Nome,
                Produto = produto.Nome,
                Quantidade = dto.Quantidade,
                ValorTotal = valorTotal
            };
        }
        catch (Exception ex)
        {
            var a = ex.Message + ex.StackTrace;
            throw;
        }
    }
}
