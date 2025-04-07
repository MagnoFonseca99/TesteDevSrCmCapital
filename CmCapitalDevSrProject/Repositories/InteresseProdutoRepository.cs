using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Repositories.Data;
using CmCapitalDevSrProject.Repositories.Interfaces;

namespace CmCapitalDevSrProject.Repositories;

public class InteresseProdutoRepository : IInteresseProdutoRepository
{
    private readonly AppDbContext _context;

    public InteresseProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(InteresseProdutoFuturo interesse)
    {
        _context.InteressesProdutosFuturos.Add(interesse);
        await _context.SaveChangesAsync();
    }
}