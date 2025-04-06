using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Repositories.Interfaces;

namespace CmCapitalDevSrProject.Services.Interfaces;

public interface IProdutoService
{
    Task<Produto?> CriarProdutoAsync(ProdutoDto dto);
    Task AtualizarProdutoAsync(int id, ProdutoDto dto);
    Task<PaginacaoResultado<ProdutoResultadoDto>> BuscarProdutosAsync(ProdutoFiltroDto filtro);

    Task AtualizarProdutoComProdutoAnteriorAsync(int id, Produto produtoAnterior, Produto produtoAtualizado);
    
    Task<List<ProdutoEstoqueBaixoDto>> ObterProdutosComEstoqueBaixoAsync(int limiteMinimo);

    Task<List<ProdutoSugestaoDto>> SugerirProdutosAsync(int clienteId, int produtoBaseId,
        IClienteRepository clienteRepository);

}