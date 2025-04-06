using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Repositories.Interfaces;

namespace CmCapitalDevSrProject.Tests.Repositories;

public class FakeProdutoRepository : IProdutoRepository
{
    public List<Produto?> Produtos { get; } = new();
    public List<ProdutoAuditoria> Auditoria { get; } = new();

    public Task<Produto?> AddAsync(Produto? produto)
    {
        produto.Id = Produtos.Count + 1;
        Produtos.Add(produto);
        return Task.FromResult(produto);
    }

    public Task<Produto?> GetByIdAsync(int id)
    {
        return Task.FromResult(Produtos.FirstOrDefault(p => p.Id == id));
    }

    public Task UpdateAsync(Produto? produto)
    {
        var index = Produtos.FindIndex(p => p.Id == produto.Id);
        if (index >= 0)
            Produtos[index] = produto;

        return Task.CompletedTask;
    }

    public Task AddAuditoriaAsync(ProdutoAuditoria auditoria)
    {
        auditoria.Id = Auditoria.Count + 1;
        Auditoria.Add(auditoria);
        return Task.CompletedTask;
    }
    
    public Task<PaginacaoResultado<ProdutoResultadoDto>> BuscarAsync(ProdutoFiltroDto filtro)
    {
        // DEBUG: Verifica produtos com Categoria nula
        var produtosComCategoriaNula = Produtos
            .Where(p => p != null && p.Categoria == null)
            .ToList();

        if (produtosComCategoriaNula.Any())
        {
            throw new Exception($"Há {produtosComCategoriaNula.Count} produto(s) com Categoria nula. IDs: {string.Join(", ", produtosComCategoriaNula.Select(p => p.Id))}");
        }
        var query = Produtos
            .Where(p => p != null)
            .Cast<Produto>()
            .AsQueryable();

        if (filtro.PrecoMin.HasValue)
            query = query.Where(p => p.Preco >= filtro.PrecoMin.Value);

        if (filtro.PrecoMax.HasValue)
            query = query.Where(p => p.Preco <= filtro.PrecoMax.Value);

        if (filtro.QuantidadeMin.HasValue)
            query = query.Where(p => p.QuantidadeEstoque >= filtro.QuantidadeMin.Value);

        if (filtro.QuantidadeMax.HasValue)
            query = query.Where(p => p.QuantidadeEstoque <= filtro.QuantidadeMax.Value);

        if (filtro.VencimentoAte.HasValue)
            query = query.Where(p => p.DataVencimento <= filtro.VencimentoAte.Value);

        var total = query.Count();

        var items = query
            .OrderBy(p => p.Nome)
            .Skip((filtro.Page - 1) * filtro.PageSize)
            .Take(filtro.PageSize)
            .Select(p => new ProdutoResultadoDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Preco = p.Preco,
                QuantidadeEstoque = p.QuantidadeEstoque,
                DataVencimento = p.DataVencimento,
                CategoriaNome = p.Categoria.Nome
            })
            .ToList();

        var resultado = new PaginacaoResultado<ProdutoResultadoDto>
        {
            TotalItems = total,
            Page = filtro.Page,
            PageSize = filtro.PageSize,
            Items = items
        };

        return Task.FromResult(resultado);
    }
    
    public Task<List<Produto>> ListarEstoqueAbaixoAsync(int limiteMinimo)
    {
        var produtosCriticos = Produtos
            .Where(p => p.QuantidadeEstoque < limiteMinimo)
            .ToList();

        return Task.FromResult(produtosCriticos);
    }
    public IQueryable<Produto> GetAll()
    {
        return Produtos
            .Where(p => p != null)
            .Cast<Produto>() // necessário pois a lista é de Produto?
            .AsQueryable();
    }
    
}