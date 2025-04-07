using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Repositories.Data;
using CmCapitalDevSrProject.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CmCapitalDevSrProject.Repositories;

public class CategoriaProdutoRepository : ICategoriaProdutoRepository
{
    private readonly AppDbContext _context;

    public CategoriaProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExisteAsync(int categoriaId)
    {
        return _context.Categorias.AnyAsync(c => c.Id == categoriaId);
    }

    public async Task<List<CategoriaProduto>> ListarTodasAsync()
    {
        return await _context.Categorias.ToListAsync();
    }

    public Task<CategoriaProduto?> ObterPorIdAsync(int id)
    {
        return _context.Categorias.FirstOrDefaultAsync(c => c.Id == id);
    }
}