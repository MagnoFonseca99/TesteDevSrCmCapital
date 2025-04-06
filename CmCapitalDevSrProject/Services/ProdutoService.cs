using System.Text.Json;
using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Repositories.Interfaces;
using CmCapitalDevSrProject.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CmCapitalDevSrProject.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<Produto> CriarProdutoAsync(ProdutoDto dto)
    {
        ValidarProduto(dto);

        var produto = new Produto
        {
            Nome = dto.Nome,
            Preco = dto.Preco,
            QuantidadeEstoque = dto.QuantidadeEstoque,
            DataVencimento = dto.DataVencimento
        };

        var criado = await _produtoRepository.AddAsync(produto);

        await _produtoRepository.AddAuditoriaAsync(new ProdutoAuditoria
        {
            ProdutoId = criado.Id,
            Operacao = "CREATE",
            DadosNovos = JsonSerializer.Serialize(produto)
        });

        return criado;
    }

    public async Task<PaginacaoResultado<ProdutoResultadoDto>> BuscarProdutosAsync(ProdutoFiltroDto filtro)
    {
        return await _produtoRepository.BuscarAsync(filtro);
    }

    public async Task AtualizarProdutoAsync(int id, ProdutoDto dto)
    {
        ValidarProduto(dto);

        var produtoAtualizado = new Produto
        {
            Nome = dto.Nome,
            Preco = dto.Preco,
            CategoriaId = dto.CategoriaId,
            QuantidadeEstoque = dto.QuantidadeEstoque,
            DataVencimento = dto.DataVencimento
        };

        await BuscarEAtualizarProdutoComAuditoriaAsync(id, produtoAtualizado);
    }

    public async Task AtualizarProdutoComProdutoAnteriorAsync(int id, Produto produtoAnterior, Produto produtoAtualizado)
    {
        await AtualizarProdutoComAuditoriaAsync(id, produtoAnterior, produtoAtualizado);
    }

    private async Task BuscarEAtualizarProdutoComAuditoriaAsync(int id, Produto produtoAtualizado)
    {
        var produtoExistente = await _produtoRepository.GetByIdAsync(id);
        if (produtoExistente == null)
            throw new ArgumentException("Produto não encontrado");

        await AtualizarProdutoComAuditoriaAsync(id, produtoExistente, produtoAtualizado);
    }

    private async Task AtualizarProdutoComAuditoriaAsync(int id, Produto produtoAnterior, Produto produtoAtualizado)
    {
        var dadosAnteriores = JsonSerializer.Serialize(produtoAnterior);

        produtoAnterior.Nome = produtoAtualizado.Nome;
        produtoAnterior.Preco = produtoAtualizado.Preco;
        produtoAnterior.CategoriaId = produtoAtualizado.CategoriaId;
        produtoAnterior.QuantidadeEstoque = produtoAtualizado.QuantidadeEstoque;
        produtoAnterior.DataVencimento = produtoAtualizado.DataVencimento;

        await _produtoRepository.UpdateAsync(produtoAnterior);

        await _produtoRepository.AddAuditoriaAsync(new ProdutoAuditoria
        {
            ProdutoId = id,
            Operacao = "UPDATE",
            DadosAnteriores = dadosAnteriores,
            DadosNovos = JsonSerializer.Serialize(produtoAnterior)
        });
    }

    private void ValidarProduto(ProdutoDto dto)
    {
        if (dto.Preco < 0)
            throw new ArgumentException("Preço não pode ser negativo");

        if (dto.QuantidadeEstoque < 0)
            throw new ArgumentException("Quantidade em estoque não pode ser negativa");

        if (dto.DataVencimento <= DateTime.Now)
            throw new ArgumentException("Data de vencimento deve ser no futuro");
    }
    
    public async Task<List<ProdutoEstoqueBaixoDto>> ObterProdutosComEstoqueBaixoAsync(int limiteMinimo)
    {
        var produtos = await _produtoRepository.ListarEstoqueAbaixoAsync(limiteMinimo);
    
        return produtos.Select(p => new ProdutoEstoqueBaixoDto
        {
            Id = p.Id,
            Nome = p.Nome,
            QuantidadeEstoque = p.QuantidadeEstoque
        }).ToList();
    }
    
    public async Task<List<ProdutoSugestaoDto>> SugerirProdutosAsync(int clienteId, int produtoBaseId, IClienteRepository clienteRepository)
    {
        var cliente = await clienteRepository.GetByIdAsync(clienteId)
                      ?? throw new ArgumentException("Cliente não encontrado");

        var produtoBase = await _produtoRepository.GetAll()
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == produtoBaseId);

        if (produtoBase == null)
            throw new ArgumentException("Produto base não encontrado");

        var vencimentoMax = produtoBase.DataVencimento.AddMonths(-4);

        var sugestoes = await _produtoRepository.GetAll()
            .Include(p => p.Categoria)
            .Where(p =>
                p.Id != produtoBase.Id &&
                p.CategoriaId == produtoBase.CategoriaId &&
                p.Preco <= cliente.SaldoDisponivel &&
                p.DataVencimento <= vencimentoMax)
            .OrderBy(p => p.Preco)
            .Take(3)
            .Select(p => new ProdutoSugestaoDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Preco = p.Preco,
                DataVencimento = p.DataVencimento,
                Categoria = p.Categoria.Nome
            })
            .ToListAsync();

        return sugestoes;
    }





    
}
