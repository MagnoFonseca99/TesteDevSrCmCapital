using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Repositories.Data;
using CmCapitalDevSrProject.Repositories.Interfaces;
using CmCapitalDevSrProject.Services.Models.DTOs;
using CmCapitalDevSrProject.Services.Utils;
using Microsoft.EntityFrameworkCore;

namespace CmCapitalDevSrProject.Repositories;

public class VendaRepository : IVendaRepository
{
    private readonly AppDbContext _context;

    public VendaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Venda> RegistrarVendaAsync(Venda venda)
    {
        _context.Vendas.Add(venda);
        await _context.SaveChangesAsync();
        return venda;
    }
    public async Task<Venda?> ObterPorIdAsync(int id)
    {
        return await _context.Vendas.FindAsync(id);
    }

    public async Task AtualizarAsync(Venda venda)
    {
        _context.Vendas.Update(venda);
        await _context.SaveChangesAsync();
    }
    public async Task<List<Venda>> BuscarVendasAsync(RelatorioVendasFiltroDto filtro)
    {
        var query = _context.Vendas
            .Include(v => v.Produto)
            .ThenInclude(p => p.Categoria)
            .AsQueryable();

        if (!filtro.IncluirEstornos)
            query = query.Where(v => !v.Estornada);

        if (filtro.DataInicio.HasValue)
            query = query.Where(v => v.DataVenda >= filtro.DataInicio.Value);

        if (filtro.DataFim.HasValue)
            query = query.Where(v => v.DataVenda <= filtro.DataFim.Value);

        if (filtro.ProdutoId.HasValue)
            query = query.Where(v => v.ProdutoId == filtro.ProdutoId.Value);

        if (filtro.CategoriaId.HasValue)
            query = query.Where(v => v.Produto.CategoriaId == filtro.CategoriaId.Value);

        return await query.ToListAsync();
    }
}