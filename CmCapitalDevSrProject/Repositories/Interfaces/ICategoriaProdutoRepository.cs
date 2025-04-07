using CmCapitalDevSrProject.Models;

namespace CmCapitalDevSrProject.Repositories.Interfaces;

public interface ICategoriaProdutoRepository
{
    Task<bool> ExisteAsync(int categoriaId);
    Task<List<CategoriaProduto>> ListarTodasAsync();
    Task<CategoriaProduto?> ObterPorIdAsync(int id);
}