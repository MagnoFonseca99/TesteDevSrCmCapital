using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Repositories.Interfaces;
using CmCapitalDevSrProject.Services.Interfaces;
using CmCapitalDevSrProject.Services.Models.DTOs;

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
        IVendaRepository vendaRepo)
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
    
    public async Task RealizarEstornoAsync(EstornoVendaDto dto)
    {
        var venda = await _vendaRepo.ObterPorIdAsync(dto.VendaId)
                    ?? throw new ArgumentException("Venda não encontrada.");

        if (venda.ClienteId != dto.ClienteId)
            throw new ArgumentException("Venda não pertence ao cliente informado.");

        if (venda.Estornada)
            throw new InvalidOperationException("Venda já foi estornada.");

        var diasDesdeCompra = (DateTime.UtcNow - venda.DataVenda).TotalDays;
        if (diasDesdeCompra > 7)
            throw new InvalidOperationException("Prazo de estorno expirado.");

        // Repor saldo e estoque
        var cliente = await _clienteRepo.GetByIdAsync(venda.ClienteId)
                      ?? throw new Exception("Cliente não encontrado.");

        var produto = await _produtoRepo.GetByIdAsync(venda.ProdutoId)
                      ?? throw new Exception("Produto não encontrado.");

        cliente.SaldoDisponivel += venda.ValorTotal;
        
        var produtoAtualizado = new Produto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Preco = produto.Preco,
            CategoriaId = produto.CategoriaId,
            DataVencimento = produto.DataVencimento,
            QuantidadeEstoque =  produto.QuantidadeEstoque + venda.Quantidade
        };
       
        venda.Estornada = true;
        venda.DataEstorno = DateTime.UtcNow;

        await _clienteRepo.UpdateAsync(venda.ClienteId, cliente);
        await _produtoService.AtualizarProdutoComProdutoAnteriorAsync(produto.Id, produto, produtoAtualizado);
        await _vendaRepo.AtualizarAsync(venda);
    }
    
    public async Task<List<RelatorioVendasItemDto>> GerarRelatorioAsync(RelatorioVendasFiltroDto filtro)
    {
        var vendas = await _vendaRepo.BuscarVendasAsync(filtro);

        var agrupado = vendas
            .GroupBy(v => new
            {
                v.Produto.Nome,
                Periodo = filtro.TipoAgrupamento.ToLower() == "anual"
                    ? v.DataVenda.Year.ToString()
                    : $"{v.DataVenda.Year:D4}-{v.DataVenda.Month:D2}"
            })
            .Select(g => new RelatorioVendasItemDto
            {
                Produto = g.Key.Nome,
                Periodo = g.Key.Periodo,
                QuantidadeTotal = g.Sum(v => v.Quantidade),
                ValorTotal = g.Sum(v => v.ValorTotal)
            })
            .OrderBy(r => r.Produto)
            .ThenBy(r => r.Periodo)
            .ToList();

        return agrupado;
    }




}
