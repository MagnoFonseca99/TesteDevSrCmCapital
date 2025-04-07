using CmCapitalDevSrProject.Repositories.Data;
using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Services.Interfaces;
using CmCapitalDevSrProject.Services.Utils;

namespace CmCapitalDevSrProject.Services;


public class CategoriaService : ICategoriaService
{
    private readonly AppDbContext _context;

    public CategoriaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoriaProduto>> ListarTodasAsync()
    {
        return await _context.Categorias.OrderBy(c => c.Nome).SafeToListAsync();
    }
}
