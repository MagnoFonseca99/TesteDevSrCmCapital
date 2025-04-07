using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Services.Models.DTOs;

namespace CmCapitalDevSrProject.Services.Interfaces;

public interface IClienteService
{
    Task<Cliente?> CriarClienteAsync(ClienteDto clienteDto);
    
    Task<Cliente?> ObterPorIdAsync(int id);

}