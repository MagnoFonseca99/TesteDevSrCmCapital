using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Repositories.Interfaces;

namespace CmCapitalDevSrProject.Tests.Fakes;

public class FakeCategoriaProdutoRepository : ICategoriaProdutoRepository
{
    public readonly List<CategoriaProduto> Categorias = new()
    {
        new CategoriaProduto { Id = 1, Nome = "Renda Fixa" }, // ✅ CategoriaId = 1
        new CategoriaProduto { Id = 2, Nome = "Renda Variável" },
        new CategoriaProduto { Id = 3, Nome = "Crédito Privado" }
    };

    public Task<bool> ExisteAsync(int id)
    {
        return Task.FromResult(Categorias.Any(c => c.Id == id));
    }

    public Task<List<CategoriaProduto>> ListarTodasAsync()
    {
        return Task.FromResult(Categorias.ToList());
    }

    public Task<CategoriaProduto?> ObterPorIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}

