using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Repositories.Interfaces;
using CmCapitalDevSrProject.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CmCapitalDevSrProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _service;
    private readonly IClienteRepository _clienteRepository;
    private readonly ILogger<ProdutosController> _logger;
    private readonly IConfiguration _config;

    public ProdutosController(
        IProdutoService service,
        IClienteRepository clienteRepository,
        ILogger<ProdutosController> logger,
        IConfiguration config)
    {
        _service = service;
        _clienteRepository = clienteRepository;
        _logger = logger;
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> CriarProduto([FromBody] ProdutoDto produtoDto)
    {
        _logger.LogInformation("POST /api/produtos - CriarProduto iniciado. Payload: {@ProdutoDto}", produtoDto);

        try
        {
            var produto = await _service.CriarProdutoAsync(produtoDto);

            _logger.LogInformation("Produto criado com sucesso. ID: {ProdutoId}", produto.Id);
            return CreatedAtAction(nameof(CriarProduto), new { id = produto.Id }, produto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erro de validação ao criar produto.");
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar produto.");
            return StatusCode(500, new { mensagem = "Erro interno no servidor. Contate o suporte." });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarProduto(int id, [FromBody] ProdutoDto produtoDto)
    {
        _logger.LogInformation("PUT /api/produtos/{Id} - AtualizarProduto iniciado. Payload: {@ProdutoDto}", id, produtoDto);

        try
        {
            await _service.AtualizarProdutoAsync(id, produtoDto);

            _logger.LogInformation("Produto atualizado com sucesso. ID: {ProdutoId}", id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Produto não encontrado ou inválido. ID: {ProdutoId}", id);
            return NotFound(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar produto. ID: {ProdutoId}", id);
            return StatusCode(500, new { mensagem = "Erro interno no servidor. Contate o suporte." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> BuscarProdutos(
        [FromQuery] decimal? precoMin,
        [FromQuery] decimal? precoMax,
        [FromQuery] int? quantidadeMin,
        [FromQuery] int? quantidadeMax,
        [FromQuery] DateTime? vencimentoAte,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var filtro = new ProdutoFiltroDto
        {
            PrecoMin = precoMin,
            PrecoMax = precoMax,
            QuantidadeMin = quantidadeMin,
            QuantidadeMax = quantidadeMax,
            VencimentoAte = vencimentoAte,
            Page = page,
            PageSize = pageSize
        };

        _logger.LogInformation("GET /api/produtos - Filtros aplicados: {@Filtro}", filtro);

        try
        {
            var resultado = await _service.BuscarProdutosAsync(filtro);

            _logger.LogInformation("Consulta de produtos concluída. Total de itens: {Total}", resultado.TotalItems);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao consultar produtos.");
            return StatusCode(500, new { mensagem = "Erro interno no servidor. Contate o suporte." });
        }
    }

    [HttpGet("estoque-baixo")]
    public async Task<IActionResult> ObterProdutosComEstoqueBaixo()
    {
        var limite = _config.GetValue<int>("Restricoes:EstoqueMinimoAlerta", 5);
        _logger.LogInformation("GET /api/produtos/estoque-baixo - Limite configurado: {Limite}", limite);

        try
        {
            var produtos = await _service.ObterProdutosComEstoqueBaixoAsync(limite);

            _logger.LogInformation("Consulta de estoque baixo retornou {Count} itens.", produtos.Count);
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos com estoque baixo.");
            return StatusCode(500, new { mensagem = "Erro interno no servidor. Contate o suporte." });
        }
    }

    [HttpGet("{produtoBaseId}/sugestoes")]
    public async Task<IActionResult> SugerirProdutos(int produtoBaseId, [FromQuery] int clienteId)
    {
        _logger.LogInformation("GET /api/produtos/{ProdutoId}/sugestoes - clienteId: {ClienteId}", produtoBaseId, clienteId);

        try
        {
            var sugestoes = await _service.SugerirProdutosAsync(clienteId, produtoBaseId, _clienteRepository);

            _logger.LogInformation("Sugestão de produtos retornou {Quantidade} itens.", sugestoes.Count);
            return Ok(sugestoes);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Parâmetros inválidos para sugestão.");
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sugerir produtos.");
            return StatusCode(500, new { mensagem = "Erro interno no servidor. Contate o suporte." });
        }
    }
}
