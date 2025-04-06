using CmCapitalDevSrProject.Models;

namespace CmCapitalDevSrProject.Repositories.Interfaces;

public interface IVendaRepository
{
    Task<Venda> RegistrarVendaAsync(Venda venda);
}