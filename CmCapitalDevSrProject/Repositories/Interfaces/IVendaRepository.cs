using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Services.Models.DTOs;

namespace CmCapitalDevSrProject.Repositories.Interfaces;

public interface IVendaRepository
{
    Task<Venda> RegistrarVendaAsync(Venda venda);
    Task<Venda?> ObterPorIdAsync(int id);
    Task AtualizarAsync(Venda venda);
    Task<List<Venda>> BuscarVendasAsync(RelatorioVendasFiltroDto filtro);



}