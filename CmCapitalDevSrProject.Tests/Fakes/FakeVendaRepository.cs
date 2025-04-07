using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Repositories.Interfaces;
using CmCapitalDevSrProject.Services.Models.DTOs;

namespace CmCapitalDevSrProject.Tests.Fakes;

public class FakeVendaRepository : IVendaRepository
{
    private readonly List<Venda> _vendas = new();

    public Task<Venda> RegistrarVendaAsync(Venda venda)
    {
        venda.Id = _vendas.Count + 1;
        _vendas.Add(venda);
        return Task.FromResult(venda);
    }

    public Task<Venda?> ObterPorIdAsync(int id)
    {
        var venda = _vendas.FirstOrDefault(v => v.Id == id);
        return Task.FromResult(venda);
    }

    public Task AtualizarAsync(Venda venda)
    {
        var index = _vendas.FindIndex(v => v.Id == venda.Id);
        if (index >= 0)
        {
            _vendas[index] = venda;
        }
        return Task.CompletedTask;
    }

    public Task<List<Venda>> BuscarVendasAsync(RelatorioVendasFiltroDto filtro)
    {
        throw new NotImplementedException();
    }

    // Auxiliar para os testes
    public List<Venda> ObterVendasRegistradas() => _vendas;
}