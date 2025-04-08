using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Services.Interfaces;
using CmCapitalDevSrProject.Services.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CmCapitalDevSrProject.Controllers;

/// <summary>
/// Endpoints para cadastro e consulta de clientes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly ILogger<ClientesController> _logger;

    public ClientesController(IClienteService clienteService, ILogger<ClientesController> logger)
    {
        _clienteService = clienteService;
        _logger = logger;
    }

    /// <summary>
    /// Cadastra um novo cliente.
    /// </summary>
    /// <param name="clienteDto">Objeto com dados do cliente.</param>
    /// <remarks>
    /// Regras de negócio:
    /// - O saldo inicial não pode ser negativo.
    /// </remarks>
    /// <returns>O cliente criado com status 201.</returns>
    /// <response code="201">Cliente criado com sucesso.</response>
    /// <response code="400">Erro de validação (ex: saldo negativo).</response>
    /// <response code="500">Erro interno no servidor.</response>
    [HttpPost(Name = "CriarCliente")]
    [ProducesResponseType(typeof(Cliente), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CriarCliente([FromBody] ClienteDto clienteDto)
    {
        _logger.LogInformation("POST /api/clientes - Requisição recebida para criação de cliente: {@ClienteDto}", clienteDto);
        try
        {
            var cliente = await _clienteService.CriarClienteAsync(clienteDto);
            _logger.LogInformation("Cliente criado com sucesso. ID: {ClienteId}", cliente.Id);
            return CreatedAtAction(nameof(ObterClientePorId), new { id = cliente.Id }, cliente);
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

    /// <summary>
    /// Busca um cliente pelo ID.
    /// </summary>
    /// <param name="id">ID do cliente.</param>
    /// <returns>Objeto do cliente, se encontrado.</returns>
    /// <response code="200">Cliente encontrado com sucesso.</response>
    /// <response code="404">Cliente não encontrado.</response>
    /// <response code="500">Erro interno no servidor.</response>
    [HttpGet("{id}", Name = "ObterClientePorId")]
    [ProducesResponseType(typeof(Cliente), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObterClientePorId(int id)
    {
        _logger.LogInformation("GET /api/clientes/{Id} - Buscando cliente por ID: {Id}", id);

        try
        {
            var cliente = await _clienteService.ObterPorIdAsync(id);
            if (cliente == null)
            {
                _logger.LogWarning("Cliente ID {Id} não encontrado.", id);
                return NotFound(new { mensagem = "Cliente não encontrado." });
            }

            return Ok(cliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar cliente ID {Id}.", id);
            return StatusCode(500, new { mensagem = "Erro interno no servidor." });
        }
    }
}
