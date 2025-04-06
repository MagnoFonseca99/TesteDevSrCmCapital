using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Repositories.Interfaces;

namespace CmCapitalDevSrProject.Tests.Repositories;

public class FakeVendaRepository : IVendaRepository
{
    private readonly List<Venda> _vendas = new();

    public Task<Venda> RegistrarVendaAsync(Venda venda)
    {
        // Simula a atribuição de um ID como o banco faria
        venda.Id = _vendas.Count + 1;
        _vendas.Add(venda);
        return Task.FromResult(venda);
    }

    // Método auxiliar para inspeção nos testes
    public List<Venda> ObterVendasRegistradas() => _vendas;
}