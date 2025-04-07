using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Repositories.Interfaces;
using CmCapitalDevSrProject.Services.Interfaces;
using CmCapitalDevSrProject.Services.Models.DTOs;

namespace CmCapitalDevSrProject.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Cliente?> CriarClienteAsync(ClienteDto clienteDto)
    {
        if (clienteDto.SaldoDisponivel < 0)
            throw new ArgumentException("Saldo inicial não pode ser menor que zero.");

        var cliente = new Cliente
        {
            Nome = clienteDto.Nome,
            SaldoDisponivel = clienteDto.SaldoDisponivel
        };

        return await _repository.AddAsync(cliente);
    }
    public async Task<Cliente?> ObterPorIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

}