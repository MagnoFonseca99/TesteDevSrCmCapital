using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Repositories.Interfaces;

namespace CmCapitalDevSrProject.Tests.Fakes;

public class FakeInteresseProdutoRepository : IInteresseProdutoRepository
{
    private readonly List<InteresseProdutoFuturo> _interesses = new();

    public Task AdicionarAsync(InteresseProdutoFuturo interesse)
    {
        interesse.Id = _interesses.Count + 1;
        _interesses.Add(interesse);
        return Task.CompletedTask;
    }

    public List<InteresseProdutoFuturo> ObterTodos() => _interesses;
}
