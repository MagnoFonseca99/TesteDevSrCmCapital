using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Services.Interfaces;
using CmCapitalDevSrProject.Services.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CmCapitalDevSrProject.Controllers;

[ApiController]
[Route("[controller]")]
public class CadastroClienteController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly ILogger<CadastroClienteController> _logger;

    public CadastroClienteController(IClienteService clienteService, ILogger<CadastroClienteController> logger)
    {
        _clienteService = clienteService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize]
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
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> ObterClientePorId(int id)
    {
        _logger.LogInformation("GET /api/clientes/{Id} - Consulta de cliente", id);

        try
        {
            var cliente = await _clienteService.ObterPorIdAsync(id);
            if (cliente == null)
            {
                return NotFound(new { mensagem = "Cliente não encontrado." });
            }

            return Ok(cliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar cliente por ID.");
            return StatusCode(500, new { mensagem = "Erro interno no servidor." });
        }
    }


}