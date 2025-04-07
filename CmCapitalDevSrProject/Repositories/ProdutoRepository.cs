using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Repositories.Data;
using CmCapitalDevSrProject.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CmCapitalDevSrProject.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> AddAsync(Produto? produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        return produto;
    }

    public async Task<Produto?> GetByIdAsync(int id)
    {
        return await _context.Produtos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task<PaginacaoResultado<ProdutoResultadoDto>> BuscarAsync(ProdutoFiltroDto filtro)
    {
        var query = _context.Produtos
            .Include(p => p.Categoria)
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

        var total = await query.CountAsync();

        var items = await query
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
            .ToListAsync();

        return new PaginacaoResultado<ProdutoResultadoDto>
        {
            TotalItems = total,
            Page = filtro.Page,
            PageSize = filtro.PageSize,
            Items = items
        };
    }
    
    public async Task<Produto?> ObterProdutoComCategoriaAsync(int id)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);
    }



    public async Task UpdateAsync(Produto? produto)
    {
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
    }

    public async Task AddAuditoriaAsync(ProdutoAuditoria auditoria)
    {
        _context.ProdutosAuditoria.Add(auditoria);
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<Produto>> ListarEstoqueAbaixoAsync(int limiteMinimo)
    {
        return await _context.Produtos
            .Where(p => p.QuantidadeEstoque < limiteMinimo)
            .ToListAsync();
    }
    
    public IQueryable<Produto?> GetAll()
    {
        return _context.Produtos.AsQueryable();
    }
}