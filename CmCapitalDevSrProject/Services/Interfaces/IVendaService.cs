using CmCapitalDevSrProject.Models.DTOs;

namespace CmCapitalDevSrProject.Services.Interfaces;

public interface IVendaService
{
    Task<VendaResultadoDto> RealizarVendaAsync(VendaDto dto, decimal saldoMinimoPermitido);
}