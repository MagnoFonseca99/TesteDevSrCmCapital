using CmCapitalDevSrProject.Models;

namespace CmCapitalDevSrProject.Services.Interfaces;

public interface ICategoriaService
{
    Task<List<CategoriaProduto>> ListarTodasAsync();
}