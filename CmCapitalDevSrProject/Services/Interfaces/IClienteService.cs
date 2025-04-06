using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;

namespace CmCapitalDevSrProject.Services.Interfaces;

public interface IClienteService
{
    Task<Cliente?> CriarClienteAsync(ClienteDto clienteDto);
}