using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CmCapitalDevSrProject.Controllers;

[ApiController]
[Route("[controller]")]
public class CadastroController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly ILogger<CadastroController> _logger;

    public CadastroController(IClienteService clienteService, ILogger<CadastroController> logger)
    {
        _clienteService = clienteService;
        _logger = logger;
        _logger.LogInformation("🔥 Teste via ILogger<ClientesController>");
    }

    [HttpPost]
    public async Task<IActionResult> CriarCliente([FromBody] ClienteDto clienteDto)
    {
        _logger.LogInformation("Requisição para criar cliente recebida: {@ClienteDto}", clienteDto);
        try
        {
            var cliente = await _clienteService.CriarClienteAsync(clienteDto);
            _logger.LogInformation("Cliente criado com sucesso: {@Cliente}", cliente);
            return CreatedAtAction(nameof(CriarCliente), new { id = cliente.Id }, cliente);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao criar cliente.");
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar cliente.");
            return StatusCode(500, new
            {
                mensagem = "Erro interno no servidor. Favor contactar a equipe de suporte."
            });
        }
    }

}