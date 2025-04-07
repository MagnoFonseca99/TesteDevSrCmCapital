using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;

namespace CmCapitalDevSrProject.Repositories.Interfaces;

public interface IProdutoRepository
{
    Task<Produto?> AddAsync(Produto? produto);
    Task<Produto?> GetByIdAsync(int id);
    Task<PaginacaoResultado<ProdutoResultadoDto>> BuscarAsync(ProdutoFiltroDto filtro);
    Task UpdateAsync(Produto? produto);
    Task AddAuditoriaAsync(ProdutoAuditoria auditoria);
    
    Task<List<Produto>> ListarEstoqueAbaixoAsync(int limiteMinimo);
    IQueryable<Produto?> GetAll(); 
    Task<Produto?> ObterProdutoComCategoriaAsync(int id);
    
    
}