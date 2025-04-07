using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Services.Models.DTOs;

namespace CmCapitalDevSrProject.Services.Interfaces;

public interface IVendaService
{
    Task<VendaResultadoDto> RealizarVendaAsync(VendaDto dto, decimal saldoMinimoPermitido);
    
    Task RealizarEstornoAsync(EstornoVendaDto dto);
    
    Task<List<RelatorioVendasItemDto>> GerarRelatorioAsync(RelatorioVendasFiltroDto filtro);


}