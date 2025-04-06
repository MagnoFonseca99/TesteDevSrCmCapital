using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Repositories.Interfaces;

namespace CmCapitalDevSrProject.Tests.Repositories;

public class FakeClienteRepository : IClienteRepository
{
    public List<Cliente?> Clientes { get; } = new();

    public Task<Cliente?> AddAsync(Cliente? cliente)
    {
        cliente.Id = Clientes.Count + 1;
        Clientes.Add(cliente);
        return Task.FromResult(cliente);
    }

    public Task<Cliente?> GetByIdAsync(int clienteId)
    {
        return Task.FromResult(Clientes.FirstOrDefault(c => c.Id == clienteId));
    }
    
    public Task<Cliente> UpdateAsync(int clienteId, Cliente clienteAtualizado)
    {
        var clienteExistente = Clientes.FirstOrDefault(c => c?.Id == clienteId);

        if (clienteExistente == null)
        {
            throw new InvalidOperationException($"Cliente com ID {clienteId} não encontrado.");
        }

        clienteExistente.Nome = clienteAtualizado.Nome;
        clienteExistente.SaldoDisponivel = clienteAtualizado.SaldoDisponivel;

        return Task.FromResult(clienteExistente);
    }
}