using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CmCapitalDevSrProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendasController : ControllerBase
{
    private readonly IVendaService _service;
    private readonly ILogger<VendasController> _logger;
    private readonly IConfiguration _config;

    public VendasController(IVendaService service, ILogger<VendasController> logger, IConfiguration config)
    {
        _service = service;
        _logger = logger;
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> RealizarVenda([FromBody] VendaDto vendaDto)
    {
        _logger.LogInformation("POST /api/vendas - RealizarVenda iniciado. Payload: {@VendaDto}", vendaDto);

        try
        {
            var saldoMinimo = _config.GetValue<decimal>("Restricoes:SaldoMinimoPermitido", 0);
            var resultado = await _service.RealizarVendaAsync(vendaDto, saldoMinimo);

            _logger.LogInformation("Venda realizada com sucesso. VendaId: {VendaId}", resultado.VendaId);
            return Ok(resultado);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao realizar venda.");
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro de regra de negócio ao realizar venda.");
            return UnprocessableEntity(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao realizar venda.");
            return StatusCode(500, new { mensagem = "Erro interno no servidor. Contate o suporte." });
        }
    }
}