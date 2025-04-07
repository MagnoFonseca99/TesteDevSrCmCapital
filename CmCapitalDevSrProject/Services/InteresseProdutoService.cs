using CmCapitalDevSrProject.Models;
using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Repositories.Interfaces;
using CmCapitalDevSrProject.Services.Interfaces;
using CmCapitalDevSrProject.Services.Models.DTOs;

namespace CmCapitalDevSrProject.Services;
public class InteresseProdutoService : IInteresseProdutoService
{
    private readonly IInteresseProdutoRepository _repo;
    private readonly IClienteRepository _clienteRepo;
    private readonly ICategoriaProdutoRepository _categoriaRepo;

    public InteresseProdutoService(
        IInteresseProdutoRepository repo,
        IClienteRepository clienteRepo,
        ICategoriaProdutoRepository categoriaRepo)
    {
        _repo = repo;
        _clienteRepo = clienteRepo;
        _categoriaRepo = categoriaRepo;
    }

    public async Task RegistrarInteresseAsync(InteresseProdutoFuturoDto dto)
    {
        if (!await _clienteRepo.ExisteAsync(dto.ClienteId))
            throw new ArgumentException("Cliente não encontrado.");

        if (dto.CategoriaId.HasValue)
        {
            if (!await _categoriaRepo.ExisteAsync(dto.CategoriaId.Value))
                throw new ArgumentException("Categoria não encontrada.");
        }

        var interesse = new InteresseProdutoFuturo
        {
            ClienteId = dto.ClienteId,
            CategoriaId = dto.CategoriaId
        };

        await _repo.AdicionarAsync(interesse);
    }
}