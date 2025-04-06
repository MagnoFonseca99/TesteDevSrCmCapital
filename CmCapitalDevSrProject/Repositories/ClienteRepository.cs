using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Repositories.Data;
using CmCapitalDevSrProject.Repositories.Interfaces;

namespace CmCapitalDevSrProject.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> AddAsync(Cliente? cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }
    
    public async Task<Cliente?> GetByIdAsync(int clienteId)
    {
        return await _context.Clientes.FindAsync(clienteId);
    }
    
    public async Task<Cliente> UpdateAsync(int clienteId, Cliente clienteAtualizado)
    {
        var clienteExistente = await _context.Clientes.FindAsync(clienteId);

        if (clienteExistente == null)
        {
            throw new InvalidOperationException($"Cliente com ID {clienteId} não encontrado.");
        }

        clienteExistente.Nome = clienteAtualizado.Nome;
        clienteExistente.SaldoDisponivel = clienteAtualizado.SaldoDisponivel;

        await _context.SaveChangesAsync();
        return clienteExistente;
    }

}