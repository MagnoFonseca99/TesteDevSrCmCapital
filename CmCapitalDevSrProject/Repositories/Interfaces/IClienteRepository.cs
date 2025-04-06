using CmCapitalDevSrProject.Models;

namespace CmCapitalDevSrProject.Repositories.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> AddAsync(Cliente? cliente);
    
    Task<Cliente?> GetByIdAsync(int clienteId);

    Task<Cliente> UpdateAsync(int clienteId, Cliente clienteAtualizado);
}