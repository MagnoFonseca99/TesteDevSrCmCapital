using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Repositories.Data;
using CmCapitalDevSrProject.Repositories.Interfaces;

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
}