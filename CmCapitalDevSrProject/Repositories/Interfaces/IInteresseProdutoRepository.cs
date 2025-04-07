using CmCapitalDevSrProject.Models;

namespace CmCapitalDevSrProject.Repositories.Interfaces;

public interface IInteresseProdutoRepository
{
    Task AdicionarAsync(InteresseProdutoFuturo interesse);
}